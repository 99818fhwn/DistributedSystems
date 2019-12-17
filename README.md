# DistributedSystems

The Protcol is best described in the ProtocollObject.cs file. 
The server dependent part is the string before the ';;' (double semicolon)
=> identifier:{will be set from the server};adapter:{the adapter this client will be using (case sensitive)};name:{just a client name};;{paramName}:{paramvalue};

So an example message will look like:

identifier:1234-5672-h1245...;adapter:exampleadapter;name:Myexampledevice;;puttext:helloworld;
