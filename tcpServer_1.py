#!/usr/bin/env python
import socket
import sys
import time
import struct
import os
from threading import Thread
from cryptography.fernet import Fernet
import base64
HOST = "192.168.137.207"  # Standard loopback interface address (localhost)
PORT = 502        # Port to listen on (non-privileged ports are > 1023)
strTemp="null"
import Adafruit_DHT
class EchoThread(Thread):
    def __init__(self, conn,strTemp):
        super().__init__()
        self.conn = conn

    def run(self):
        print()
        print(f"Echoing {self.conn}")
        numLines = 0
        
        while True:
            data = self.conn.recv(1024)
            shared_key = Fernet.generate_key()
            f= Fernet(shared_key)
            print(shared_key)
            if not data:
                print("end of data, stopping")
                return
            strData= data.decode("utf-8")
            print("str:",strData)
            #if(strData=="key"):
                #self.conn.sendall(shared_key)
                #return
            if(strData=="Read"):
                print("str:",strData)
                humidity, temperature = Adafruit_DHT.read_retry(11, 4)
                humidity=humidity+0.25
                temperature=temperature+0.25
                print(humidity, temperature)
                strHum= str(humidity)
                strTemp=str(temperature)
                Separator=","
                Separator=Separator.encode()
                strHum= strHum.encode()
                strTemp= strTemp.encode()
                sendData=strHum+Separator+strTemp
                cipher_token = f.encrypt(sendData)
                print(cipher_token)
                self.conn.sendall(cipher_token+Separator+shared_key)
                return
           #strTemp= strData.split("-")
           # print("temp:", strTemp[0])
            #self.conn.sendall(data)
            #numLines += 1
            #print(f"received data from {self.conn}")
            #print(f"  data: {data}, count: {numLines}")

def main(host, port):
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

    s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    s.bind((host, port))
    s.listen()

    while True:
        print("Main thread waiting for connections")
        srvConn, addr = s.accept()
        eThread = EchoThread(srvConn,strTemp)
        print(f"connection from {addr}, spawning echo thread {eThread.name}")
        eThread.start()

main(HOST, PORT)
