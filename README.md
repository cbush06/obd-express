# OBD Express
This application provides a simplue and intuitive GUI for reading diagnostic data, error codes, and other system information from you vehicle. My long-term goal is to also add the capability for interacting with some vehicle systems (resetting the trouble light, manufacturer specific features, etc.), providing a bus dump/scan feature for passively monitoring all bus transmissions, and providing a trip logging feature that works with GPS and Google Maps.

# Solution Structure
This solution presently includes 6 individual projects:
* __ELM327API__: This is the API for interfacing with an ELM327-like device, reading and processing diagnostics data from the OBD/bus, and transmitting any commands to the OBD/bus.
* __ObdExpress__: The actual GUI interface that uses the ELM327API project to monitor the OBD/bus and display data.
* __BasicHandlers__: Handlers that process OBD messages relating to the most commonly supported information on vehicles (includes various temperature readings, engine RPM, and VIN Number).
* __EngineDemandHandlers__: Handlers that process OBD messages specifically related to engine demand (includes engine load, throttle position, pedal position, and torque).
* __DiagnosticHandlers__: Handlers related to the vehicle's malfunction detection system (includes MIL lamp, ECU DTC counter, and others).
* __MiscHanlders__: Handlers that process messages not used by most users (includes handlers for messages related specifically to pollution control systems).

# Modularity
OBD Express is designed to be a modular application. The application is structured so that it is a trivial task to add additional protocol support. Currently, OBD Express only supports __11-bit ID CAN messages__. Support for __29-bit ID CAN messages__ will be added soon. I hope that other protocols will follow.

OBD Express has also made modularity a first-class consideration regarding OBD message support. Additional Message Handlers can be can be bundled as DLLs and added as _plug-ins_ within the application.
