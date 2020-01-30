# Distributed Systems
# Server - Client architecture
Example of use: Motion sensor (connected to Esp32) activating a sound(or other) actuator (connected to Esp8266) when motion detected via defined pipeline. 

Every device should define an Adapter for the server and put  .dll in the ./WebSocketExample/wwwroot/adapters

The Protcol is best described in the ProtocollObject.cs file. 
The server dependent part is the string before the ';;' (double semicolon)
=> identifier:{will be set from the server};adapter:{the adapter this client will be using (case sensitive)};name:{just a client name};;{paramName}:{paramvalue};

So an example message will look like:

identifier:1234-5672-h1245...;adapter:exampleadapter;name:Myexampledevice;;puttext:helloworld;

The connected devices have to send a message in an intervall under 5 minutes in order to stay connected to the server.
