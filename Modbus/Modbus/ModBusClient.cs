using System;
using System.Net.Sockets;

namespace Modbus
{
    public class ModBusClient : IModBusClient
    {
        public ModBusClient(string ipAddress, int port)
        {
            IpAddress = ipAddress;
            Port = port;
        }

        private string IpAddress { get; }
        private int Port { get; }
        public TcpClient TcpClient { get; set; }
        public NetworkStream Stream { get; set; }
        private byte Identifier { get; } = 0x01;
        private byte FunctionCode { get; set ; }
        private byte[] ReceiveData { get; set ; }
        private byte[] Length { get; } = BitConverter.GetBytes((int) 0x0006);
        private byte[] ProtocolIdentifier { get; } = BitConverter.GetBytes((int) 0x0000);
        private byte[] TransactionIdentifier { get; } = BitConverter.GetBytes((uint) 1);
        private byte[] Crc { get; } = new byte[2];

        public void Connect()
        {
            Console.WriteLine("Connecting: IP-Address: " + IpAddress + ", Port: " + Port, System.DateTime.Now);
            TcpClient = new TcpClient();
            var result = TcpClient.BeginConnect(IpAddress, Port, null, null);
            var success = result.AsyncWaitHandle.WaitOne(1000);
            if (!success)
            {
                throw new Exception("connection timed out");
            }
            
            if (TcpClient.Connected)
            {
                Console.WriteLine("Is connected");
            }
            
            Stream = TcpClient.GetStream();
            Stream.ReadTimeout = 1000;
        }

        public void Disconnect()
        {
            Stream.Close();         
            TcpClient.Close();
            if (!TcpClient.Connected)
            {
                Console.WriteLine("Is disconnected");
            }
        }

        public bool ReadCoil(int address)
        {
            address--;
            bool[] response;
            var startingAddress = BitConverter.GetBytes(address);
            var quantity = BitConverter.GetBytes(1);
            FunctionCode = 0x01;
            Byte[] data = new byte[]
            {
                TransactionIdentifier[1],
                TransactionIdentifier[0],
                ProtocolIdentifier[1],
                ProtocolIdentifier[0],
                Length[1],
                Length[0],
                Identifier,
                FunctionCode,
                startingAddress[1],
                startingAddress[0],
                quantity[1],
                quantity[0],
                Crc[1],
                Crc[0]
            };

            Stream.Write(data, 0, data.Length - 2);

            data = new Byte[2100];

            int NumberOfBytes = Stream.Read(data, 0, data.Length);
            ReceiveData = new byte[NumberOfBytes];
            Array.Copy(data, 0, ReceiveData, 0, NumberOfBytes);


            response = new bool[1];
            for (int i = 0; i < 1; i++)
            {
                int intData = data[9 + i / 8];
                int mask = Convert.ToInt32(Math.Pow(2, (i % 8)));
                response[i] = Convert.ToBoolean((intData & mask) / mask);
            }

            return response[0];
        }
        
        public int ReadRegister(int address)
        {
            address--;
            int[] response;
            var startingAddress = BitConverter.GetBytes(address);
            var quantity = BitConverter.GetBytes(1);
            FunctionCode = 0x03;
            Byte[] data = new byte[]
            {
                TransactionIdentifier[1],
                TransactionIdentifier[0],
                ProtocolIdentifier[1],
                ProtocolIdentifier[0],
                Length[1],
                Length[0],
                Identifier,
                FunctionCode,
                startingAddress[1],
                startingAddress[0],
                quantity[1],
                quantity[0],
                Crc[1],
                Crc[0]
            };
                
            Stream.Write(data, 0, data.Length-2);
                
            data = new Byte[2100];
            int NumberOfBytes = Stream.Read(data, 0, data.Length);
            
            ReceiveData = new byte[NumberOfBytes];
            Array.Copy(data, 0, ReceiveData, 0, NumberOfBytes);
            
            response = new int[2];
            for (int i = 0; i < 2; i++)
            {
                byte lowByte;
                byte highByte;
                highByte = data[9+i*2];
                lowByte = data[9+i*2+1];
				
                data[9+i*2] = lowByte;
                data[9+i*2+1] = highByte;
				
                response[i] = BitConverter.ToInt16(data,(9+i*2));
            }

            return response[0];
        }

        public void WriteCoil(int address, bool value)
        {
            address--;
            var startingAddress = BitConverter.GetBytes(address);
            FunctionCode = 0x05;
            byte[] coilValue = new byte[2];
            
            if (value == true)
            {
                coilValue = BitConverter.GetBytes((int)0xFF00);
            }
            else
            {
                coilValue = BitConverter.GetBytes((int)0x0000);
            }
            
            Byte[] data = new byte[]
            {
                TransactionIdentifier[1],
                TransactionIdentifier[0],
                ProtocolIdentifier[1],
                ProtocolIdentifier[0],
                Length[1],
                Length[0],
                Identifier,
                FunctionCode,
                startingAddress[1],
                startingAddress[0],
                coilValue[1],
                coilValue[0],
                Crc[1],
                Crc[0]
            };
            
            Stream.Write(data, 0, data.Length - 2);
            
            data = new Byte[2100];
            int NumberOfBytes = Stream.Read(data, 0, data.Length);
            ReceiveData = new byte[NumberOfBytes];
            Array.Copy(data, 0, ReceiveData, 0, NumberOfBytes);
        }

        public void WriteRegister(int address, int value)
        {
            address--;
            var startingAddress = BitConverter.GetBytes(address);
            FunctionCode = 0x06;
            byte[] registerValue = new byte[2];
            registerValue = BitConverter.GetBytes((int)value);
            
            Byte[] data = new byte[]
            {
                TransactionIdentifier[1],
                TransactionIdentifier[0],
                ProtocolIdentifier[1],
                ProtocolIdentifier[0],
                Length[1],
                Length[0],
                Identifier,
                FunctionCode,
                startingAddress[1],
                startingAddress[0],
                registerValue[1],
                registerValue[0],
                Crc[1],
                Crc[0]
            };
            
            Stream.Write(data, 0, data.Length - 2);
            
            data = new Byte[2100];
            int NumberOfBytes = Stream.Read(data, 0, data.Length);
            ReceiveData = new byte[NumberOfBytes];
            Array.Copy(data, 0, ReceiveData, 0, NumberOfBytes);
        }
    }
}