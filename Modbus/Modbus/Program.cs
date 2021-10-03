using System;

namespace Modbus
{
    class Program
    {
        static void Main(string[] args)
        {
            string ipAddress = "127.0.0.1";
            int port = 502;

            ModBusClient modBusClient = new ModBusClient(ipAddress, port);
            modBusClient.Connect();

           
            Console.WriteLine("Coil 1 before change : " +  modBusClient.ReadCoil(1));
            modBusClient.WriteCoil(1, true);
            modBusClient.WriteCoil(6, true);
            modBusClient.WriteCoil(7, true);
            modBusClient.WriteCoil(8, true);
            Console.WriteLine("Coil 1 after change  : " +  modBusClient.ReadCoil(1));
            
            
            Console.WriteLine("Coil 4  : " +  modBusClient.ReadCoil(4));
            Console.WriteLine("Coil 9  : " +  modBusClient.ReadCoil(9));
            Console.WriteLine("Register 1 before change : " +  modBusClient.ReadRegister(1));
            modBusClient.WriteRegister(1, 33);
            modBusClient.WriteRegister(7, 1111);
            modBusClient.WriteRegister(8, 00);
            modBusClient.WriteRegister(9, 5335);
            Console.WriteLine("Register 1 after change : " +  modBusClient.ReadRegister(1));
            
            
            Console.WriteLine("Register 3  : " +  modBusClient.ReadRegister(3));
            Console.WriteLine("Register 5  : " +  modBusClient.ReadRegister(5));
            
            modBusClient.Disconnect();
        }
    }
}