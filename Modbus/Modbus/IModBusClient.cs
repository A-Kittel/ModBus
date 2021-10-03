namespace Modbus
{
    public interface IModBusClient
    {
        void Connect();
        void Disconnect();
        bool ReadCoil(int address);
        int ReadRegister(int address);
        void WriteCoil(int address, bool value);
        void WriteRegister(int address, int value);
    }
}