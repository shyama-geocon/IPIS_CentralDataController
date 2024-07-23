using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;

namespace IpisCentralDisplayController.Helpers
{
    public class SqliteDBJsonHelper
    {
        private readonly string _connectionString;

        public SqliteDBJsonHelper(string databasePath)
        {
            _connectionString = $"Data Source={databasePath};Version=3;";
            if (!File.Exists(databasePath))
            {
                SQLiteConnection.CreateFile(databasePath);
                InitializeDatabase();
            }
        }

        private void InitializeDatabase()
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                string createTableQuery = @"
                    CREATE TABLE IF NOT EXISTS JsonData (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Key TEXT NOT NULL,
                        Json TEXT NOT NULL
                    )";
                using (var command = new SQLiteCommand(createTableQuery, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        public void Save<T>(string key, T value)
        {
            string json = JsonConvert.SerializeObject(value);
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                string insertQuery = "INSERT INTO JsonData (Key, Json) VALUES (@Key, @Json)";
                using (var command = new SQLiteCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@Key", key);
                    command.Parameters.AddWithValue("@Json", json);
                    command.ExecuteNonQuery();
                }
            }
        }

        public T Load<T>(string key)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                string selectQuery = "SELECT Json FROM JsonData WHERE Key = @Key";
                using (var command = new SQLiteCommand(selectQuery, connection))
                {
                    command.Parameters.AddWithValue("@Key", key);
                    var result = command.ExecuteScalar() as string;
                    if (string.IsNullOrEmpty(result))
                    {
                        return default;
                    }
                    return JsonConvert.DeserializeObject<T>(result);
                }
            }
        }

        public void Delete(string key)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                string deleteQuery = "DELETE FROM JsonData WHERE Key = @Key";
                using (var command = new SQLiteCommand(deleteQuery, connection))
                {
                    command.Parameters.AddWithValue("@Key", key);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void Update<T>(string key, T value)
        {
            string json = JsonConvert.SerializeObject(value);
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                string updateQuery = "UPDATE JsonData SET Json = @Json WHERE Key = @Key";
                using (var command = new SQLiteCommand(updateQuery, connection))
                {
                    command.Parameters.AddWithValue("@Key", key);
                    command.Parameters.AddWithValue("@Json", json);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
