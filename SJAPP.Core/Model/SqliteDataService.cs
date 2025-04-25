using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;

namespace SJAPP.Core.Model
{
    public class SqliteDataService
    {
        private readonly string dbPath;

        public SqliteDataService(string subFolder = "Data", string databaseName = "SJ_data.db")
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string dataDirectory = Path.Combine(baseDirectory, subFolder);
            if (!Directory.Exists(dataDirectory))
            {
                Directory.CreateDirectory(dataDirectory);
            }
            dbPath = Path.Combine(dataDirectory, databaseName);
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            try
            {
                if (!File.Exists(dbPath))
                {
                    using (File.Create(dbPath)) { }
                }

                using (var connection = new SqliteConnection($"Data Source={dbPath}"))
                {
                    connection.Open();
                    // 創建 DeviceData 表，儲存設備的所有配置
                    string sql = @"
                        CREATE TABLE IF NOT EXISTS DeviceData (
                            Id INTEGER PRIMARY KEY,
                            Name TEXT,
                            IpAddress TEXT,
                            SlaveId INTEGER,
                            IsOperational INTEGER,
                            RunCount INTEGER
                        )";
                    using (var command = new SqliteCommand(sql, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to initialize database: {ex.Message}", ex);
            }
        }

        public void SaveDeviceData(int deviceId, string name, string ipAddress, int slaveId, bool isOperational, int runCount)
        {
            try
            {
                using (var connection = new SqliteConnection($"Data Source={dbPath}"))
                {
                    connection.Open();
                    string sql = @"
                        INSERT OR REPLACE INTO DeviceData (Id, Name, IpAddress, SlaveId, IsOperational, RunCount)
                        VALUES (@Id, @Name, @IpAddress, @SlaveId, @IsOperational, @RunCount)";
                    using (var command = new SqliteCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Id", deviceId);
                        command.Parameters.AddWithValue("@Name", name);
                        command.Parameters.AddWithValue("@IpAddress", ipAddress);
                        command.Parameters.AddWithValue("@SlaveId", slaveId);
                        command.Parameters.AddWithValue("@IsOperational", isOperational ? 1 : 0);
                        command.Parameters.AddWithValue("@RunCount", runCount);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to save device data: {ex.Message}", ex);
            }
        }

        public List<(string Name, string IpAddress, int SlaveId, bool IsOperational, int RunCount)> GetDeviceData()
        {
            try
            {
                var deviceData = new List<(string, string, int, bool, int)>();
                using (var connection = new SqliteConnection($"Data Source={dbPath}"))
                {
                    connection.Open();
                    string sql = "SELECT Name, IpAddress, SlaveId, IsOperational, RunCount FROM DeviceData ORDER BY Id";
                    using (var command = new SqliteCommand(sql, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var name = reader.IsDBNull(0) ? null : reader.GetString(0);
                                var ipAddress = reader.IsDBNull(1) ? null : reader.GetString(1);
                                var slaveId = reader.IsDBNull(2) ? 0 : reader.GetInt32(2);
                                var isOperational = reader.IsDBNull(3) ? true : reader.GetInt32(3) == 1;
                                var runCount = reader.IsDBNull(4) ? 0 : reader.GetInt32(4);
                                deviceData.Add((name, ipAddress, slaveId, isOperational, runCount));
                            }
                        }
                    }
                }
                return deviceData;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to retrieve device data: {ex.Message}", ex);
            }
        }

        public string GetDatabasePath()
        {
            return dbPath;
        }
    }
}