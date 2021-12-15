#!/usr/bin/python
# Copyright (c) 2014 Adafruit Industries
# Author: Tony DiCola

# Permission is hereby granted, free of charge, to any person obtaining a copy
# of this software and associated documentation files (the "Software"), to deal
# in the Software without restriction, including without limitation the rights
# to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
# copies of the Software, and to permit persons to whom the Software is
# furnished to do so, subject to the following conditions:

# The above copyright notice and this permission notice shall be included in all
# copies or substantial portions of the Software.

# THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
# IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
# FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
# AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
# LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
# OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
# SOFTWARE.
import sys
import time

import Adafruit_DHT
import time
from azure.iot.device import IoTHubDeviceClient, Message
import json
from datetime import datetime
CONNECTION_STRING = "HostName=secure-crop-temp-iot-sym.azure-devices.net;DeviceId=secure-crop-temp-pa;SharedAccessKey=+Wmn75lrjPLuWFyH9S0XrZy1lMaOM9fzuT4mq4e1PjQ="

# Parse command line parameters.
sensor_args = { '11': Adafruit_DHT.DHT11,
                '22': Adafruit_DHT.DHT22,
                '2302': Adafruit_DHT.AM2302 }
if len(sys.argv) == 3 and sys.argv[1] in sensor_args:
    sensor = sensor_args[sys.argv[1]]
    pin = sys.argv[2]
else:
    print('Usage: sudo ./Adafruit_DHT.py [11|22|2302] <GPIO pin number>')
    print('Example: sudo ./Adafruit_DHT.py 2302 4 - Read from an AM2302 connected to GPIO pin #4')
    sys.exit(1)

# Try to grab a sensor reading.  Use the read_retry method which will retry up
# to 15 times to get a sensor reading (waiting 2 seconds between each retry).

UNIT = 0x1
## Create a shared key and send it to Modbus server
#shared_key = Fernet.generate_key()
#print(shared_key)
#bytesShared_key= shared_key.hex()
#print(bytesShared_key)
#Write it to Modbus Register
#rq = client.write_register(2, bytesShared_key, unit=UNIT)
def iothub_client_init():
    #client = IoTHubDeviceClient(CONNECTION_STRING)
    client=IoTHubDeviceClient.create_from_connection_string(CONNECTION_STRING)
    print("iothub-client-initiated")
    return client
def iothub_client_telemetry_sample_run():
    print("telemetry connection initated");
    


def main(host, port):
    sensor="PA-Sensor"
    strSensor= str(sensor)
    sensor_args = { '11': Adafruit_DHT.DHT11,
                '22': Adafruit_DHT.DHT22,
                '2302': Adafruit_DHT.AM2302 }
    if len(sys.argv) == 3 and sys.argv[1] in sensor_args:
        sensor = sensor_args[sys.argv[1]]
        pin = sys.argv[2]
    else:
        print('Usage: sudo ./Adafruit_DHT.py [11|22|2302] <GPIO pin number>')
        print('Example: sudo ./Adafruit_DHT.py 2302 4 - Read from an AM2302 connected to GPIO pin #4')
        sys.exit(1)

    client=iothub_client_init()
     

    while True:
        print("Temp Data")
        humidity, temperature = Adafruit_DHT.read_retry(11, 4)
        print(humidity, temperature)
        intTemp= int(temperature)
        intTemp=(intTemp*9/5)+32
        print("temp in F", intTemp)
        strHumid= str(humidity)
        date_time = datetime.now().strftime("%m-%d-%YT%H:%M:%S")
        msg = {"time" : date_time, "dspl" : strSensor, "temp" : intTemp}
        msg = Message(json.dumps(msg))
        msg.content_encoding = "utf-8"
        msg.content_type = "application/json"
        client.send_message(msg)
        print("Message Successfully sent")
        time.sleep(10)

main(11, 4)
while(True):
    humidity, temperature = Adafruit_DHT.read_retry(sensor, pin)
    print(humidity, temperature)
    date_time = datetime.now().strftime("%m-%d-%YT%H:%M:%S")
    print(str(humidity), str(temperature))

# Un-comment the line below to convert the temperature to Fahrenheit.
# temperature = temperature * 9/5.0 + 32

# Note that sometimes you won't get a reading and
# the results will be null (because Linux can't
# guarantee the timing of calls to read the sensor).
# If this happens try again!
