using SJAPP.Core.Model;
using System;
using System.Threading.Tasks;

namespace SJAPP.Core.Services.Communication
{
    public interface ICommunicationService
    {

        Task<ModbusReadResult> ReadModbusAsync(string ip, int slaveId, int address, int quantity, int functionCode);
        Task<string> WriteModbusAsync(string ip, int slaveId, int address, int value);
        Task<string> SendLedCommandAsync(string ip, string state);
        void CleanupConnections();
    }
}