#include <WiFi.h>
#include <WebSocketClient.h>

// this is a test
// real program: when sensor notices movenent it notifies (5V or else)
// on this event method "send data to client" will be called with data 1

//---------- pin
const int sensorPin = 4; // io4

//--------- make dynamic?
const char* ssid = "Obi-WLAN-Kenobi"; //"root";//"ALERTA-P1b"; //"ALERTA3"; // 
const char* password = "Anacardo01"; //"fox4025652000";//"sd98f7sdSD98F7SD"; // 

//------- change ! ip from the laptop where server is running
const uint16_t port = 5000;
char* path = "/ws";
char* host = "192.168.0.59"; //"192.168.10.56"; // "192.168.0.102";
int numOfTries = 0;

WebSocketClient webSocketClient;
WiFiClient client;
String adapter = "motionsensoradapter";
String adapterName = "MotionSensorAdapter";
String identifier;
int previousParameterValue = 0;
int parameterValue = 0; // ---> the i/o pin value = parameterValue <<<<<<<<<<<<<<<<<<< 
// <<<<<<<<<<<<<<<<< if there is a change in io pin  -> update and send data to server
String data;
String responsce;
int timer = 0;

void setup()
{
    pinMode(sensorPin, INPUT);
    Serial.begin(115200);

    WiFi.begin(ssid, password);
    while (WiFi.status() != WL_CONNECTED) 
    {
      delay(500);
      Serial.println("Connecting to WiFi...");
    }

    Serial.print("WiFi connected with IP: ");
    Serial.println(WiFi.localIP());

    //---- connect to server 
    connectToServer();
    delay(500);
}

void loop()
{
    // checks the input/output pin (the one where the sensor is connected)
    // if its state changed sends it to server as parameter (state:1 || state:0) if not sends nothing
    // interaption pins?

    // ------ use device id and data as parameters
    // CheckIfStateChanged();
    Serial.println("looping ......................");

    if (client.connected())
    {
        // get identifier if have none
        if(identifier.length() == 0)
        {
            Serial.println("Getting id: ");
            data = "identifier:;adapter:"+adapter+";name:"+adapterName+";;isOn:" + parameterValue +";puttext:gettingID;";
            webSocketClient.sendData(data);

            delay(350);

            webSocketClient.getData(responsce);

            if (responsce.length() > 0) 
            {
                Serial.print("Received data: ");
                Serial.println(responsce);

                identifier = GetIdentifier(responsce);

                Serial.print("identifier: ");
                Serial.println(identifier);
            }
            else
            {
                Serial.println("responsce is null");
            }

            delay(500);
        }
        else
        {
            timer += 1;
            Serial.print("timer: "); 
            Serial.println(timer); 

            Serial.println("sending data: ");            

            //check if sensor changed data
            if(CheckIfStateChanged())
            {
                Serial.print("state changed: ");
                Serial.println(parameterValue);
            }
                data = "identifier:" + identifier + ";adapter:"+adapter+";name:"+adapterName+";;isOn:" + parameterValue +";puttext:valuechanged;";
                webSocketClient.sendData(data);
                
                delay(500);

                webSocketClient.getData(responsce);

                if (responsce.length() > 0) 
                {
                    if(identifier == GetIdentifier(responsce))
                    {
                        // get the received parameters 
                        int newParameterValue = GetParameterValue(responsce).toInt();     
                        Serial.print("Received parameter value: ");
                        Serial.println(responsce); 
                        Serial.println(newParameterValue);           
                        
                        // if(parameterValue != newParameterValue && newParameterValue >= 0)
                        // {             
                        //     // send data to server
                        //     data = "identifier:" + identifier + ";adapter:"+adapter+";name:"+adapterName+";;isOn:" + parameterValue +";";
                        //     webSocketClient.sendData(data);
                        // }
                    }
                }
                else
                {
                    Serial.println("responsce is null");
                }
            // }
            // else
            // {
            //     data = "identifier:" + identifier + ";adapter:"+adapter+";name:"+adapterName+";;isOn:" + parameterValue +";";
            //     webSocketClient.sendData(data);
            //     Serial.print("state didnt change: ");
            //     Serial.println(parameterValue);
            // }
        }
    }
    else 
    {
        Serial.println("Client disconnected.");
        // identifier = "";
        //---- connect to server 
        connectToServer();
    }

    // if(timer >= 30) // random value
    // {
    //     timer = 0;
    //     Serial.println("reconnecting to server");
        
    //     data = "identifier:" + identifier + ";adapter:"+adapter+";name:"+adapterName+";;deleteConn:1;reconnection:"+identifier+";";
    //     webSocketClient.sendData(data);
    //     delay(2000);
    //     connectToServer();
    //     data = "identifier:" + identifier + ";adapter:"+adapter+";name:"+adapterName+";;reconnection:"+identifier+";";
    //     delay(50);
    //     webSocketClient.sendData(data);
    //     delay(2000);
    // }

    delay(1000);
}

bool CheckIfStateChanged()
{
    Serial.print("old data: ");
    Serial.println(previousParameterValue);

    parameterValue = digitalRead(sensorPin);
    Serial.print("new data: ");
    Serial.println(parameterValue);

    if(parameterValue != previousParameterValue)
    {
        previousParameterValue = parameterValue;
        Serial.println("returning true");
        return true;
    }
    else
    {
        Serial.println("returning false");
        return false;
    }
}


void connectToServer()
{
    if (!client.connect(host, port)) 
    {
        Serial.print("Connection to host failed ");
        Serial.println(numOfTries);
        
        numOfTries += 1;

        delay(1000);
        return;
    }

    Serial.println("Connected to server successful!");
    // ---------- websocket handshake

    webSocketClient.path = path;
    webSocketClient.host = host;

    if (webSocketClient.handshake(client)) 
    {
        Serial.println("Handshake successful");
    } 
    else 
    {
        Serial.println("Handshake failed.");
    }
}

String GetParameterValue(String input)
{
    bool parameters = false;
    bool writingMode = false;
    String parametervalue = "";

    // ";adapter:motionsensoradapter;name:MotionSensorAdapter;;isOn:1;"
    
    for(int i = 0; i < input.length(); i++)
    {
        if(i > 0 && input[i - 1] == ';' && input[i] == ';')
        {
            parameters = true;
        }

        if(parameters)
        {
            if(input[i] == ':')
            {
                writingMode = true;
                continue;
            }
        }
        

        if(writingMode)
        {
            if(input[i] == ';')
            {
                writingMode = false;
                break;
            }

            parametervalue += input[i];
        }
    }

    return parametervalue;
}

String GetIdentifier(String input)
{
    // "identifier:fhdfdhdh;adapter:"+adapter+";name:"+adapterName+";;puttext:testmsg;";
    bool writingMode = false;
    String id = "";

    for(int i = 0; i < input.length(); i++)
    {
        if(input[i] == ';')
        {
            writingMode = false;
            break;
        }

        if(writingMode)
        {
            id += input[i];
        }

        if(input[i] == ':')
        {
            writingMode = true;
        }
    }

    return id;
}
            
            