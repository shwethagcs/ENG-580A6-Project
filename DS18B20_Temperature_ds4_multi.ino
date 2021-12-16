#include <OneWire.h>
#include <DallasTemperature.h>
/***
 * Author: Brandon O'Briant
 * Date: 10/22/2021
 * Description: This program reads multiple sensor data from 1-wire bus 
 *              using Dallas Temperature 1-Wire library and was heavily adapted from the references below.
 * References: 
 *  1. Last Minute Engineers: DS18B20 Arduino Tutorial: Reading Multiple Sensor Data via Dallas 1-Wire Bus
 */



// Data wire is plugged into digital pin 2 on the Arduino
#define ONE_WIRE_BUS 2

// Setup a oneWire instance to communicate with any OneWire device
OneWire oneWire(ONE_WIRE_BUS);  

// Pass oneWire reference to DallasTemperature library
DallasTemperature sensors(&oneWire);

int deviceCount = 0;
float tempC;

void setup(void)
{
  sensors.begin();  // Start up the library
  Serial.begin(9600);
  deviceCount = sensors.getDeviceCount();

}

void loop(void)
{ 
  // Send command to all the sensors for temperature conversion
  sensors.requestTemperatures(); 
   
  // Display temperature from each sensor
  for (int i = 0;  i < deviceCount;  i++)
  {
    Serial.print("Sensor ");
    Serial.print(i+1);
    Serial.print(" ");
    tempC = sensors.getTempCByIndex(i);
    Serial.print(DallasTemperature::toFahrenheit(tempC));
    Serial.println("F");
    delay(1000);
  }
  
  //Serial.println("");
  delay(1000);
}
