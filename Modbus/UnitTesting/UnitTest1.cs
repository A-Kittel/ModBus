using Modbus;
using Xunit;

namespace UnitTesting
{
    public class UnitTest1
    {
        string ipAddress = "127.0.0.1";
        int port = 502;
        [Fact]
        public void CheckConnection()
        {
            ModBusClient client = new ModBusClient(ipAddress, port);
            client.Connect();
            Assert.True(client.TcpClient.Connected);
            client.Disconnect();
        }
        
        [Fact]
        public void CheckDisconnection()
        {
            ModBusClient client = new ModBusClient(ipAddress, port);
            client.Connect();
            client.Disconnect();
            Assert.False(client.TcpClient.Connected);
        }
        
        [Fact]
        public void CheckWriteAndReadCoil()
        {
            ModBusClient client = new ModBusClient(ipAddress, port);
            client.Connect();
            client.WriteCoil(99, true);
            var checkCoil = client.ReadCoil(99);
            Assert.True(checkCoil);
            client.WriteCoil(99, false);
            checkCoil = client.ReadCoil(99);
            client.Disconnect();
            Assert.False(checkCoil);
        }
        
        [Fact]
        public void CheckWriteAndReadRegister()
        {
            ModBusClient client = new ModBusClient(ipAddress, port);
            client.Connect();
            var checkValue = 777;
            client.WriteRegister(99, checkValue);
            var checkRegister = client.ReadRegister(99);
            client.Disconnect();
            Assert.Equal(checkValue, checkRegister);
        }
    }
}