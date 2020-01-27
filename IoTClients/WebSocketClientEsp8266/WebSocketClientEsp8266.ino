#include <ESP8266WiFi.h>
#include <WebSocketClient.h>

// this is a test
// real program: when sensor notices movenent it notifies (5V or else)
// on this event method "send data to client" will be called with data 1

//---------- pin
const int actuatorPin = 4; // D2 pin

//--------- make dynamic?
const char* ssid = "Obi-WLAN-Kenobi"; //"root";//"ALERTA-P1b"; //"ALERTA3"; // 
const char* password = "Anacardo01"; // "sd98f7sdSD98F7SD"; //

//------- change ! ip from the laptop where server is running
const uint16_t port = 5000;
char* path = "/ws";
char* host = "192.168.0.59"; //"192.168.10.56"; //"192.168.43.5"; // // "192.168.0.102";
int numOfTries = 0;

WebSocketClient webSocketClient;
WiFiClient client;
String identifier;
String adapter = "soundactuatoradapter";
String adapterName = "soundaActuatorAdapter";
int previousParameterValue = 0;
int parameterValue = 0; // ---> the i/o pin value = parameterValue <<<<<<<<<<<<<<<<<<< 
// <<<<<<<<<<<<<<<<< if there is a change in io pin  -> update and send data to server
String data;
String responsce;


void setup()
{
    pinMode(actuatorPin, OUTPUT);
    Serial.begin(115200);

    SetActuatorValue(parameterValue);
    
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


    if (client.connected())
    {
        // get identifier if have none
        if(identifier.length() == 0)
        {
            Serial.print("Getting id: ");
            data = "identifier:;adapter:"+adapter+";name:"+adapterName+";;puttext:gettingID;";
            webSocketClient.sendData(data);

            delay(350);

            webSocketClient.getData(responsce);

            if (data.length() > 0) 
            {
                Serial.print("Received data: ");
                Serial.println(responsce);

                //SetIdentifier(data);
                identifier = GetIdentifier(responsce);

                Serial.print("identifier: ");
                Serial.println(identifier);
            }
            else
            {
                Serial.println("responsce is null");
            }

            delay(1000);
        }
        else
        {
            Serial.println("requesting data: ");
            // get data from server and if changed update pin
            // data = "identifier:" + identifier + ";adapter:"+adapter+";name:"+adapterName+";;isOn:" + parameterValue +";puttext:requestingValue;";
            // webSocketClient.sendData(data);

            

            webSocketClient.getData(responsce);
            delay(50);

            //webSocketClient.sendData(data);

            if (responsce.length() > 0) 
            {
                if( identifier == GetIdentifier(responsce))
                {
                    
                    Serial.print("Received data: ");
                    Serial.println(responsce);

                    int newValue = GetParameterValue(responsce).toInt();
                    if (CheckIfStateChanged(newValue))
                    {
                        Serial.print("Data changed from: ");
                        Serial.println(parameterValue);
                        Serial.print("To: ");
                        Serial.println(newValue);

                        parameterValue = newValue;
                        data = "identifier:" + identifier + ";adapter:"+adapter+";name:"+adapterName+";;isOn:" + parameterValue +";puttext:valuechanged;";
                        webSocketClient.sendData(data);
                        SetActuatorValue(parameterValue);
                    }
                }
            }
            else
            {
                // data = "identifier:" + identifier + ";adapter:"+adapter+";name:"+adapterName+";;isOn:" + parameterValue +";puttext:valuechanged;";
                // webSocketClient.sendData(data);
                Serial.println("responsce is null");
                delay(1000);
            }
        }
    }
    else 
    {
        Serial.println("Client disconnected.");
        // identifier = "";
        //---- connect to server 
        connectToServer();
    }

    delay(500);
}

bool CheckIfStateChanged(int newParameterValue)
{
    if(newParameterValue != parameterValue)
    {
        return true;
    }
    else
    {
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

    // ";adapter:"+adapter+";name:"+adapterName+";;isOn:1;"
    
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

    String t((const __FlashStringHelper*) "true");
    String f((const __FlashStringHelper*) "false");

    String p = parametervalue;
    p.toLowerCase();

    if(p == t)
    {
        parametervalue = "1";
    }
    else if(p == f)
    {
        parametervalue = "0";
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

void SetActuatorValue(int parameterValue)
{
    if(parameterValue > 0)
    {
        Serial.print("Setting pin value HIGH: ");
        digitalWrite(actuatorPin, HIGH);
    }
    else
    {
        Serial.print("Setting pin value LOW");
        digitalWrite(actuatorPin, LOW);
    }

    Serial.println(parameterValue);
}
            
            