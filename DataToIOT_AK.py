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
import serial
import time
from azure.iot.device import IoTHubDeviceClient, Message
import json
import datetime



CONNECTION_STRING = "<<CHANGE CONNECTION STRING TO ONE PROVIDED FROM AZURE IOT HUB - PRIMARY CONNECTION STRING TO USE SYMMETRIC KEY ENCRYPTION"
ser=serial.Serial("/dev/ttyACM0",9600)
ser.flush()
while True:
    def iothub_client_init():
        client = IoTHubDeviceClient.create_from_connection_string(CONNECTION_STRING, websockets=True)
        print("iothub-client-initiated")
        return client
    def iothub_client_telemetry_sample_run():
        print("telemetry function started")                                                                                                                                                                                                                                                               
        try:
            client = iothub_client_init()
            print("sending data to IoT Hub, plress Ctrl-c to exit")
            while True:
                if ser.in_waiting > 0:
                    date_time_str = datetime.datetime.now().isoformat()
                    tmp = str(ser.readline().rstrip(),"utf-8")
                    tmp = tmp.rstrip(tmp[-1])
                    sensor = "AK " + tmp[0:8]
                    temp = float(tmp[9::])
                    print(date_time_str + " : " + sensor + " " + str(temp))
                    msg = {"time" : date_time_str, "dspl" : sensor, "temp" : temp}
                    msg = Message(json.dumps(msg))
                    msg.content_encoding = "utf-8"
                    msg.content_type = "application/json"
                    client.send_message(msg)
                    print("Message Successfully sent")
        except KeyboardInterrupt:
            print("IoTHubClient stopped")

    if __name__ == '__main__':
        print("Press Ctrl-c to stop")
        iothub_client_telemetry_sample_run()  

 
