using ELM327API;
using ELM327API.Global;
using ELM327API.Processing.DataStructures;
using ELM327API.Processing.Interfaces;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ELM327API.Processing.Protocols
{
    class CAN_11_Bit_ISO15765_Protocol : IProtocol
    {
        /// <summary>
        /// Allow the watchdog to monitor our response times.
        /// </summary>
        public override event NoReturnWithLongParam BroadcastResponseTime;

        /// <summary>
        /// Buffer responses. Use a Hashtable because some requests will receive responses from a variety
        /// of modules on the network. We can hash the responses by CAN ID (source address).
        /// </summary>
        private Dictionary<uint, Response> _responseBuffer = new Dictionary<uint, Response>(4);

        /// <summary>
        /// Stores the ELM 327's responses to requests.
        /// </summary>
        private StringBuilder _incomingResponseBuffer = new StringBuilder(30, 60);

        /// <summary>
        /// Used to indicate when it's time to quit listening for responses
        /// </summary>
        private bool _hasMoreResponses = false;

        /// <summary>
        /// Used to update the current running time for the watchdog
        /// </summary>
        private Stopwatch stopwatch = new Stopwatch();

        /*
         * Bit masks used for extracting data from the data frame header (assuming we capture the 3 most significant bytes - or 24 bits - as the header).
         */
        private readonly ushort BITMASK_ID          = 0xFFE0; // CAN ID
        private readonly ushort BITMASK_RTR         = 0x0001; // Remote Transmission Request
        private readonly ushort BITMASK_IDE         = 0x0001; // Identifier Extension
        private readonly ushort BITMASK_FRM_TYPE    = 0x0001; // Frame Type
        private readonly ushort BITMASK_DLC         = 0x000F; // Data Length Code
        private readonly ushort BITMASK_CRC         = 0xFFFF; // Cyclic Redundancy Code
        private readonly ushort BITMASK_ACK         = 0x0003; // Acknowledge
        private readonly ushort BITMASK_EOF         = 0x007F; // End of Frame
        private readonly ushort BITMASK_IFS         = 0x007F; // Interframe Space
        private readonly ushort BITMASK_PCITYPE     = 0x000F; // Protocol Control Information (Frame Type: 0 = Single Frame, 1 = First Frame, 2 = Consecutive Frame, 3 = Flow Control)
        private readonly ushort BITMASK_FF_DLC      = 0x00FF; // First Frame Data Length Code
        private readonly ushort BITMASK_CF_SN       = 0x000F; // Consecutive Frame Sequence Number
        private readonly ushort BITMASK_SID         = 0x00BF; // Service Identifier
        private readonly ushort BITMASK_PID         = 0x00FF; // Parameter Identifier

        /*
         * Standard error messages for ELM327.
         */
        private string noDataMessage = "NO DATA";
        private string searchingMessage = "SEARCHING...";
        private string unableConnectMessage = "UNABLE TO CONNECT";
        private string stoppedMessage = "STOPPED";

        /// <summary>
        /// Enum designating the different types of frames.
        /// </summary>
        private enum FrameTypes
        {
            SINGLE_FRAME = 0,
            FIRST_FRAME = 1,
            CONSECUTIVE_FRAME = 2,
            FLOW_CONTROL = 3
        }

        // Store the most recently used custom header. If we keep track of which header was used last, and sequential frames use the same custom header,
        // we will not need to set/reset the header every transmission. This saves TIME!
        private StringBuilder _lastUsedHeader = new StringBuilder(30, 50);

        /// <summary>
        /// Executes one request/response cycle on the port set as this Protocol's connection.
        /// </summary>
        /// <param name="handler">The handler for whom the request response cycle is handled for.</param>
        /// <returns>True if the execution was successful; otherwise, false.</returns>
        public unsafe override bool Execute(IHandler handler)
        {
            // Variables
            uint idBuffer = 0x00000000;
            uint pciBuffer = 0x00000000;
            uint i = 0;
            Response curResponse;
            bool returnValue = true;
            string tempString;

            // Single Frame Data Buffer
            byte[] parsingBuffer;
            byte[] multiframeDataBuffer;

            /*
             * --  STEPS  --
             * 1. Set/Clear Headers
             * 2. Transmit request
             * 3. Process responses
             */

            // Reset our stopwatch
            this.CurrentRunningTime = 0;
            this.stopwatch.Reset();

            // Reset stop execution flag
            this.StopExecution = false;

            // If this handler requires custom headers, let's take care of that
            if (handler.IsCustomHeader)
            {
                // Only set the custom header if it is not already set
                if (!(this._lastUsedHeader.Equals(handler.Header)))
                {
                    // Request the Semaphore
                    this.ConnectionSemaphore.WaitOne();

                    // Set header
                    this._incomingResponseBuffer.Append(this.ExecuteATCommand(@"SH" + handler.Header));
                    if (!(this._incomingResponseBuffer.ToString().Equals("OK")))
                    {
                        log.Error("Attempt at Set Headers [AT SH " + handler.Header + "] failed. Response: " + this._incomingResponseBuffer.ToString());
                        return false;
                    }
                    this._incomingResponseBuffer.Clear();

                    // Release the Semaphore
                    this.ConnectionSemaphore.Release();

                    this._lastUsedHeader.Clear();
                    this._lastUsedHeader.Append(handler.Header);
                }
            }
            // If this handler does not require custom headers, but the last handler did,
            // we MUST clear the custom headers out of the ELM 327 device.
            else
            {
                // If _lastUsedHeader 
                if (!(this._lastUsedHeader.ToString().Equals("")))
                {
                    // Request the Semaphore
                    this.ConnectionSemaphore.WaitOne();

                    // Restore ELM 327 to defaults
                    this._incomingResponseBuffer.Append(this.ExecuteATCommand(@"D"));
                    if (!(this._incomingResponseBuffer.ToString().Equals("OK")))
                    {
                        log.Error("Attempt at Set to Defaults [AT D] failed. Response: " + this._incomingResponseBuffer.ToString());
                        return false;
                    }
                    this._incomingResponseBuffer.Clear();

                    // Release the Semaphore
                    this.ConnectionSemaphore.Release();

                    this._lastUsedHeader.Clear();
                }
            }

            // Transmit the request
            this.ConnectionSemaphore.WaitOne();
            this.Connection.DiscardInBuffer();
            this.Connection.DiscardOutBuffer();
            this.ExecuteCommand(handler.Request);

            // Do we expect a response?
            if (handler.ExpectsResponse)
            {
                // Begin listening
                this._hasMoreResponses = true;

                try
                {
                    this.stopwatch.Start();
                    while (this._hasMoreResponses && !(this.StopExecution))
                    {
                        // Clear the buffer
                        this._incomingResponseBuffer.Clear();

                        // Grab the incoming message
                        this._incomingResponseBuffer.Append(this.Connection.ReadLine());

                        // Verify a line was received
                        if (this._incomingResponseBuffer.Length < 1)
                        {
                            continue;
                        }

                        // Remove the prompt if it exists
                        if (this._incomingResponseBuffer[0] == '>')
                        {
                            this._incomingResponseBuffer.Remove(0, 1);
                        }

                        // Ensure an error was not received
                        tempString = this._incomingResponseBuffer.ToString();
                        if (tempString.Equals(stoppedMessage) || tempString.Equals(noDataMessage) || tempString.Equals(searchingMessage) || tempString.Equals(unableConnectMessage))
                        {
                            continue;
                        }

                        // Convert the hex string to actual bits
                        parsingBuffer = Utility.HexStringToByteArray(tempString);
                        
                        fixed (byte* p1 = parsingBuffer)
                        {
                            // Get the CAN ID
                            idBuffer = (uint)(((parsingBuffer[0] << 8) | parsingBuffer[1]) & BITMASK_ID) >> 5;

                            // Get the PCI Code
                            pciBuffer = (uint)parsingBuffer[1] & BITMASK_PCITYPE;

                            switch (pciBuffer)
                            {
                                case (uint)FrameTypes.SINGLE_FRAME:
                                {
                                    try
                                    {
                                        // Send the data payload to the handler
                                        handler.ProcessResponse(
                                            ProcessSingleFrameResponse(handler, parsingBuffer)
                                        );

                                        // Exit gracefully
                                        this._hasMoreResponses = false;
                                    }
                                    catch (ProtocolProcessingException e)
                                    {
                                        log.Error("Single Frame Processing Exception Occurred for [" + handler.Name + ":" + handler.Request + "]:", e);
                                        returnValue = false;
                                    }
                                    break;
                                }

                                case (uint)FrameTypes.FIRST_FRAME:
                                {
                                    try
                                    {
                                        // Process the First Frame. The data bytes and other session information
                                        // for this multiframe response will be stored in the _responseBuffer.
                                        ProcessFirstFrameResponse(idBuffer, handler, parsingBuffer);
                                    }
                                    catch (ProtocolProcessingException e)
                                    {
                                        log.Error("First Frame Processing Exception Occurred for [" + handler.Name + ":" + handler.Request + "]:", e);

                                        // Cancel this Request/Response Cycle
                                        this._responseBuffer.Remove(idBuffer);
                                        returnValue = false;
                                    }
                                    break;
                                }

                                case (uint)FrameTypes.CONSECUTIVE_FRAME:
                                {
                                    try
                                    {
                                        // Process this frame
                                        multiframeDataBuffer = ProcessConsecutiveFrameResponse(idBuffer, handler, parsingBuffer);

                                        // We're done, so let's deliver the data to the interested Handler
                                        if (multiframeDataBuffer != null)
                                        {
                                            // Send the data to the handler
                                            handler.ProcessResponse(multiframeDataBuffer);

                                            // Remove this response from the response buffer
                                            this._responseBuffer.Remove(idBuffer);
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        log.Error("Consecutive Frame Processing Exception Occurred for [" + handler.Name + ":" + handler.Request + "]:", e);

                                        // Cancel this Request/Response Cycle
                                        this._responseBuffer.Remove(idBuffer);
                                        returnValue = false;
                                    }

                                    break;
                                }
                            }
                        }

                        // Update the running time
                        this.CurrentRunningTime = stopwatch.ElapsedMilliseconds;

                        // Check if we need to continue listening
                        if (this._responseBuffer.Count == 0)
                        {
                            this._hasMoreResponses = false;
                        }

                        // Clear the buffer to try again
                        this._incomingResponseBuffer.Clear();
                    }
                }
                catch (Exception e)
                {
                    log.Error("Exception occurred while reading responses for [" + handler.Name + ":" + handler.Request + "]:", e);
                    returnValue = false;
                }
                
                // Stop the stopwatch, clear the current running time, and broadcast the response time
                this.stopwatch.Stop();
                this.CurrentRunningTime = 0;

                if (this.BroadcastResponseTime != null)
                {
                    this.BroadcastResponseTime(this.stopwatch.ElapsedMilliseconds);
                }
            }

            // Release the Semaphore
            this.ConnectionSemaphore.Release();

            return returnValue;
        }

        /// <summary>
        /// Process a Single Frame response and return the data payload of the response packet.
        /// </summary>
        /// <param name="handler">Handler requesting the response.</param>
        /// <param name="packetBuffer">Byte buffer with the raw bytes of the received response.</param>
        /// <returns>Byte buffer with the data payload extracted from the response.</returns>
        private byte[] ProcessSingleFrameResponse(IHandler handler, byte[] packetBuffer)
        {
            // Variables
            uint dlBuffer = 0x00000000;
            uint pidBuffer = 0x00000000;
            uint sidBuffer = 0x00000000;
            uint dataOffset = 0, i = 0;
            byte[] returnValue = null;

            try
            {
                // Get data length
                dlBuffer = (uint)(packetBuffer[2] >> 4) & BITMASK_DLC;

                // Verify Data Length
                if (dlBuffer == 0 || dlBuffer > 7)
                {
                    log.Error("Received CAN Single Frame with invalid Data Length [" + dlBuffer.ToString() + "].");
                    this._hasMoreResponses = false;
                    
                    throw new ProtocolProcessingException("Received CAN Single Frame with invalid Data Length [" + dlBuffer.ToString() + "].");
                }

                // Verify OBD (if applicable)
                if (handler.IsOBD)
                {
                    // SAE Standard OBD (2 databytes)
                    sidBuffer = (uint)((packetBuffer[2] << 4) | (packetBuffer[3] >> 4)) & BITMASK_SID;
                    pidBuffer = (uint)((packetBuffer[3] << 4) | (packetBuffer[4] >> 4)) & BITMASK_PID;

                    if (!(handler.OBDSID == sidBuffer))
                    {
                        log.Error("Response to OBD request [" + handler.Request + "] has invalid SID [" + sidBuffer + "]. It should be [" + handler.OBDSID + "].");
                        throw new ProtocolProcessingException("Response to OBD request [" + handler.Request + "] has invalid SID [" + sidBuffer + "]. It should be [" + handler.OBDSID + "].");
                    }

                    if (!(handler.OBDPID == pidBuffer))
                    {
                        log.Error("Response to OBD request [" + handler.Request + "] has invalid PID [" + pidBuffer + "]. It should be [" + handler.OBDPID + "].");
                        throw new ProtocolProcessingException("Response to OBD request [" + handler.Request + "] has invalid PID [" + pidBuffer + "]. It should be [" + handler.OBDPID + "].");
                    }

                    dataOffset = 2;
                }
                else
                {
                    dataOffset = 0;
                }

                // Create buffer
                returnValue = new byte[dlBuffer - dataOffset];

                // Fill buffer -- We have to do extra work because of the strange offset our 11-bit CAN ID
                for (i = 0; i < (dlBuffer - dataOffset); i++)
                {
                    returnValue[i] = (byte)(((packetBuffer[i + dataOffset + 2] & 0x0F) << 4) | ((packetBuffer[i + dataOffset + 3] & 0xF0) >> 4));
                }
            }
            catch (Exception e)
            {
                log.Error("Exception occurred while reading single frame response to command [" + handler.Request + "].", e);
                throw new ProtocolProcessingException("Exception occurred while reading single frame response to command [" + handler.Request + "].", e);
            }

            return returnValue;
        }

        /// <summary>
        /// Process a First Frame response and store the session information for the multiframe response in a Response object.
        /// </summary>
        /// <param name="canId">The CAN ID of the sending ECU.</param>
        /// <param name="handler">Handler requesting the response.</param>
        /// <param name="packetBuffer">Byte buffer with the raw bytes of the received response.</param>
        private void ProcessFirstFrameResponse(uint canId, IHandler handler, byte[] packetBuffer)
        {
            // Variables
            uint dlBuffer = 0x00000000;
            uint pidBuffer = 0x00000000;
            uint sidBuffer = 0x00000000;
            uint dataOffset = 0, i = 0;
            Response response = new Response();

            try
            {
                // Get data length
                dlBuffer = (uint)((packetBuffer[2] << 4) | (packetBuffer[3] >> 4));

                // Validate data length
                if (dlBuffer < 8)
                {
                    log.Error("Invalid data length received for first frame. Data Length = " + dlBuffer.ToString());
                    throw new ProtocolProcessingException("Invalid data length received for first frame. Data Length = " + dlBuffer.ToString());
                }

                // Verify OBD (if applicable)
                if (handler.IsOBD)
                {
                    // Verify SID and PID
                    sidBuffer = (uint)((packetBuffer[3] << 4) | (packetBuffer[4] >> 4)) & BITMASK_SID;
                    pidBuffer = (uint)((packetBuffer[4] << 4) | (packetBuffer[5] >> 4)) & BITMASK_PID;

                    if (!(handler.OBDSID == sidBuffer))
                    {
                        log.Error("Response to OBD request [" + handler.Request + "] has invalid SID [" + sidBuffer + "]. It should be [" + handler.OBDSID + "].");
                        throw new ProtocolProcessingException("Response to OBD request [" + handler.Request + "] has invalid SID [" + sidBuffer + "]. It should be [" + handler.OBDSID + "].");
                    }

                    if (!(handler.OBDPID == pidBuffer))
                    {
                        log.Error("Response to OBD request [" + handler.Request + "] has invalid PID [" + sidBuffer + "]. It should be [" + handler.OBDPID + "].");
                        throw new ProtocolProcessingException("Response to OBD request [" + handler.Request + "] has invalid PID [" + sidBuffer + "]. It should be [" + handler.OBDPID + "].");
                    }

                    dataOffset = 2;
                }
                else
                {
                    dataOffset = 0;
                }

                // Set data length. Subtract the 2 bytes for the SID and PID
                response.DataLength = dlBuffer - 2;

                // Create the data buffer
                response.Data = new byte[response.DataLength];

                // Read the first 4 bytes of data (a normal addressing FF has 6, a 29-bit ID CAN FF has 3)
                for (i = 0; i < 4; i++)
                {
                    response.Data[i] = (byte)(((packetBuffer[i + dataOffset + 3] & 0x0F) << 4) | ((packetBuffer[i + dataOffset + 4] & 0xF0) >> 4));
                }

                // Set the received frames and data length
                response.ReceivedFrames = 1;
                response.ReceivedLength = 4;

                // Store the response for appending its consecutive frames when they arrive
                if(this._responseBuffer.ContainsKey(canId))
                {
                    this._responseBuffer.Remove(canId);
                }
                this._responseBuffer.Add(canId, response);
            }
            catch (Exception e)
            {
                log.Error("Exception occurred while reading first frame response to command [" + handler.Request + "].", e);
                throw new ProtocolProcessingException("Exception occurred while reading first frame response to command [" + handler.Request + "].", e);
            }
        }

        /// <summary>
        /// Process a Consecutive Frame response and return the data payload of the response packet if this is the final response. If it is not
        /// the final response, store the session information for the multiframe response in a Response object.
        /// </summary>
        /// <param name="canId">The CAN ID of the sending ECU.</param>
        /// <param name="handler">Handler requesting the response.</param>
        /// <param name="packetBuffer">Byte buffer with the raw bytes of the received response.</param>
        /// <return>The data payload if this is the final response packet; otherwise, null.</return>
        private byte[] ProcessConsecutiveFrameResponse(uint canId, IHandler handler, byte[] packetBuffer)
        {
            // Variables
            uint snBuffer = 0x00000000;
            uint i = 0;
            Response response = null;

            try
            {
                // Retrieve the response this frame is for
                if (!(this._responseBuffer.ContainsKey(canId)))
                {
                    log.Error("Unidentified consecutive frame received.");
                    throw new ProtocolProcessingException("Unidentified consecutive frame received.");
                }
                response = this._responseBuffer[canId];

                // Get the sequence number
                snBuffer = (uint)packetBuffer[2] >> 4;

                // Validate sequence number
                if ((response.ReceivedFrames % 15) != snBuffer)
                {
                    log.Error("Invalid sequence number received for frame. Frames Received = " + response.ReceivedFrames.ToString() + "; Sequence Number = " + snBuffer.ToString());
                    throw new ProtocolProcessingException("Invalid sequence number received for frame. Frames Received = " + response.ReceivedFrames.ToString() + "; Sequence Number = " + snBuffer.ToString());
                }

                // Handle an intermediate frame (normal addressing CF can contain 7 bytes, or 6 for 29-bit ID CAN CF)
                if ((response.DataLength - response.ReceivedLength) > 7)
                {
                    for (i = 0; i < 7; i++)
                    {
                        response.Data[response.ReceivedLength + i] = (byte)(((packetBuffer[i + 2] & 0x0F) << 4) | ((packetBuffer[i + 3] & 0xF0) >> 4));
                    }

                    // Update our Current Response values
                    response.ReceivedLength += 7;
                    response.ReceivedFrames += 1;
                }
                // Handle the final frame
                else
                {
                    for (i = 0; i < (response.DataLength - response.ReceivedLength); i++)
                    {
                        response.Data[response.ReceivedLength + i] = (byte)(((packetBuffer[i + 2] & 0x0F) << 4) | ((packetBuffer[i + 3] & 0xF0) >> 4));
                    }

                    return response.Data;
                }
            }
            catch (Exception e)
            {
                log.Error("Exception occurred while reading consecutive frame response to command [" + handler.Request + "].", e);
                throw new ProtocolProcessingException("Exception occurred while reading consecutive frame response to command [" + handler.Request + "].", e);
            }

            return null;
        }
    }
}
