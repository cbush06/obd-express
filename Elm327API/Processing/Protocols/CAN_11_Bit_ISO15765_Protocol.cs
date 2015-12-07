using ELM327API.Global;
using ELM327API.Processing.DataStructures;
using ELM327API.Processing.Handlers;
using ELM327API.Processing.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

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
        /// List of most common ECUs.
        /// </summary>
        public override List<ECU> EcuAddresses {
            get { return _EcuAddresses;  }
        }
        private List<ECU> _EcuAddresses = new List<ECU>()
        {
            new ECU("Broadcast", 0, 0x7DF, 0x000),
            new ECU("Engine Control Module", 1, 0x7E0, 0x7E8),
            new ECU("Transmission Control Module", 2, 0x7E1, 0x7E9),
            new ECU("ECU #3", 3, 0x7E2, 0x7EA),
            new ECU("ECU #4", 4, 0x7E3, 0x7EB),
            new ECU("ECU #5", 5, 0x7E4, 0x7EC),
            new ECU("ECU #6", 6, 0x7E5, 0x7ED),
            new ECU("ECU #7", 7, 0x7E6, 0x7EE),
            new ECU("ECU #8", 8, 0x7E7, 0x7EF)
        };

        /// <summary>
        /// ECU selected for addressing requests and filtering responses.
        /// </summary>
        private ECU _selectedEcuFilter = Global.Constants.NoSelection;
        public override ECU SelectedEcuFilter
        {
            get { return _selectedEcuFilter; }
            set { _selectedEcuFilter = value; }
        }

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
            CurrentRunningTime = 0;
            stopwatch.Reset();

            // Reset stop execution flag
            StopExecution = false;
            
            // If no ECU filter is set and this handler requires custom headers, let's take care of that
            if (SelectedEcuFilter == Global.Constants.NoSelection && handler.IsCustomHeader)
            {
                // Only set the custom header if it is not already set
                if (!(_lastUsedHeader.Equals(handler.Header)))
                {
                    if (!SetRequestHeader(handler.Header)) return false;
                    _lastUsedHeader.Clear().Append(handler.Header);
                }
            }
            // If no custom handler is required or an ECU filter is selected...
            else
            {
                // If an ECU filter is selected, apply it if necessary
                if(SelectedEcuFilter != Global.Constants.NoSelection)
                {
                    // Skip this if we're debugging (the emulator doesn't handle the AT SH command)
                    #if !DEBUG
                    if(!(_lastUsedHeader.ToString().Equals(SelectedEcuFilter.RequestAddressString)))
                    {
                        if (!SetRequestHeader(SelectedEcuFilter.RequestAddressString)) return false;
                        _lastUsedHeader.Clear().Append(SelectedEcuFilter.RequestAddressString);
                    }
                    #endif
                }
                // If no ECU filter is applied and this handler does not require custom headers, 
                // but the last handler did, we MUST clear the custom headers out of the ELM 327 device.
                else if (!(_lastUsedHeader.ToString().Equals("")))
                {
                    if (!SetRequestHeader(EcuAddresses[0].RequestAddressString)) return false;
                    _lastUsedHeader.Clear();
                    _incomingResponseBuffer.Clear();
                }
            }

            // Transmit the request
            ConnectionSemaphore.WaitOne();
            Connection.DiscardInBuffer();
            Connection.DiscardOutBuffer();
            ExecuteCommand(handler.Request);

            // Do we expect a response?
            if (handler.ExpectsResponse)
            {
                // Begin listening
                _hasMoreResponses = true;

                try
                {
                    stopwatch.Start();
                    while (_hasMoreResponses && !(StopExecution))
                    {
                        // Clear the buffer
                        _incomingResponseBuffer.Clear();

                        // Grab the incoming message
                        _incomingResponseBuffer.Append(Connection.ReadLine());

                        // Verify a line was received
                        if (_incomingResponseBuffer.Length < 1)
                        {
                            continue;
                        }

                        // Remove the prompt if it exists
                        if (_incomingResponseBuffer[0] == '>')
                        {
                            _incomingResponseBuffer.Remove(0, 1);
                        }

                        // Ensure an error was not received
                        tempString = _incomingResponseBuffer.ToString();
                        if (tempString.Equals(Global.Constants.STOPPED_MESSAGE) || tempString.Equals(Global.Constants.NO_DATA_MESSAGE) || tempString.Equals(Global.Constants.SEARCHING_MESSAGE) || tempString.Equals(Global.Constants.UNABLE_TO_CONNECT_MESSAGE))
                        {
                            continue;
                        }

                        // Convert the hex string to actual bits
                        parsingBuffer = Utility.HexStringToByteArray(tempString);
                        
                        fixed (byte* p1 = parsingBuffer)
                        {
                            // Get the CAN ID (we grab 12 bits to compare the 3 hex characters -- note, the last bit is always 0)
                            idBuffer = (uint)(((parsingBuffer[0] << 8) | parsingBuffer[1]) & BITMASK_ID) >> 4;

                            // Get the PCI Code
                            pciBuffer = (uint)parsingBuffer[1] & BITMASK_PCITYPE;

                            // If an ECU filter is set, confirm the idBuffer equals the ECU's Return Address
                            if(SelectedEcuFilter != Global.Constants.NoSelection && idBuffer != SelectedEcuFilter.ReturnAddress)
                            {
                                log.Error("Filtering for [" + SelectedEcuFilter.ReturnAddressString + "]. Message from [" + idBuffer.ToString("X6") + "] rejected.");
                                returnValue = false;
                                break;
                            }

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
                                            _hasMoreResponses = false;
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
                                            _responseBuffer.Remove(idBuffer);
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
                                                _responseBuffer.Remove(idBuffer);
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        log.Error("Consecutive Frame Processing Exception Occurred for [" + handler.Name + ":" + handler.Request + "]:", e);

                                            // Cancel this Request/Response Cycle
                                            _responseBuffer.Remove(idBuffer);
                                        returnValue = false;
                                    }

                                    break;
                                }
                            }
                        }

                        // Update the running time
                        CurrentRunningTime = stopwatch.ElapsedMilliseconds;

                        // Check if we need to continue listening
                        if (_responseBuffer.Count == 0)
                        {
                            _hasMoreResponses = false;
                        }

                        // Clear the buffer to try again
                        _incomingResponseBuffer.Clear();
                    }
                }
                catch (Exception e)
                {
                    log.Error("Exception occurred while reading responses for [" + handler.Name + ":" + handler.Request + "]: " + e.Message);
                    returnValue = false;
                }

                // Stop the stopwatch, clear the current running time, and broadcast the response time
                stopwatch.Stop();
                CurrentRunningTime = 0;

                if (BroadcastResponseTime != null)
                {
                    BroadcastResponseTime(stopwatch.ElapsedMilliseconds);
                }
            }

            // Release the Semaphore
            ConnectionSemaphore.Release();

            return returnValue;
        }

        /// <summary>
        /// This method cycles through the list of ECU Addresses (see <see cref="IProtocol.EcuAddresses"/>) and
        /// queries each one to see if it exists. A list of responsive ECUs is returned.
        /// </summary>
        /// <returns>List of ECUs that responded to queries.</returns>
        public override List<ECU> AutoDetectEcus()
        {
            // Variables
            List<ECU> returnValue = new List<ECU>();
            ELM327ListenerEventArgs response = null;
            Stopwatch stopWatch = new Stopwatch();

            // Find a Handler with the provided name
            DiagnosticSessionControlHandler interrogationHandler = new DiagnosticSessionControlHandler();

            // Add an anonymous listener that gets the value for us
            interrogationHandler.RegisterSingleListener(
                new Action<ELM327ListenerEventArgs>(
                    (ELM327ListenerEventArgs args) =>
                    {
                        response = args;
                    }
                )
            );

            // Wait no more than 100 milliseconds for a response
            foreach (ECU nextEcu in EcuAddresses)
            {
                // Set the ECU request address
                interrogationHandler.Header = nextEcu.RequestAddress.ToString("X6");

                // Interrogate
                bool successfulExecution = Execute(interrogationHandler);

                // If a valid response was received, add the ECU
                if (successfulExecution && response != null && !response.IsBadResponse)
                {
                    returnValue.Add(nextEcu);
                }

                response = null;
            }

#if DEBUG
            // If we are debugging, add the Engine Control Module as a "responsive" ECU
            // because the emulator doesn't recognize UDS commands (see ISO15765-3).
            returnValue.Add(EcuAddresses[1]);
#endif

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
                    _hasMoreResponses = false;
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
                        throw new ProtocolProcessingException("Response to OBD request [" + handler.Request + "] has invalid SID [" + sidBuffer + "]. It should be [" + handler.OBDSID + "].");
                    }

                    if (!(handler.OBDPID == pidBuffer))
                    {
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
                        throw new ProtocolProcessingException("Response to OBD request [" + handler.Request + "] has invalid SID [" + sidBuffer + "]. It should be [" + handler.OBDSID + "].");
                    }

                    if (!(handler.OBDPID == pidBuffer))
                    {
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
                if(_responseBuffer.ContainsKey(canId))
                {
                    _responseBuffer.Remove(canId);
                }
                _responseBuffer.Add(canId, response);
            }
            catch (Exception e)
            {
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
                if (!(_responseBuffer.ContainsKey(canId)))
                {
                    throw new ProtocolProcessingException("Unidentified consecutive frame received.");
                }
                response = _responseBuffer[canId];

                // Get the sequence number
                snBuffer = (uint)packetBuffer[2] >> 4;

                // Validate sequence number
                if ((response.ReceivedFrames % 15) != snBuffer)
                {
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
                throw new ProtocolProcessingException("Exception occurred while reading consecutive frame response to command [" + handler.Request + "].", e);
            }

            return null;
        }
    }
}
