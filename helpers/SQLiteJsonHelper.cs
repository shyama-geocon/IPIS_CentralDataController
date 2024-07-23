using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq.Expressions;
using Newtonsoft.Json;

namespace IpisCentralDisplayController.Helpers
{
    public class SQLiteJsonHelper
    {
        private readonly string _connectionString;

        public SQLiteJsonHelper(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Save<T>(string tableName, T value)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                var json = JsonConvert.SerializeObject(value);
                var command = new SQLiteCommand($"INSERT INTO {tableName} (JsonData) VALUES (@JsonData)", connection);
                command.Parameters.AddWithValue("@JsonData", json);
                command.ExecuteNonQuery();
            }
        }

        public T Load<T>(string tableName, Expression<Func<T, bool>> predicate)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                var command = new SQLiteCommand($"SELECT JsonData FROM {tableName}", connection);
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    var json = reader.GetString(0);
                    var item = JsonConvert.DeserializeObject<T>(json);
                    if (predicate.Compile()(item))
                    {
                        return item;
                    }
                }
            }
            return default;
        }

        public void Delete<T>(string tableName, Expression<Func<T, bool>> predicate)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                var command = new SQLiteCommand($"SELECT Id, JsonData FROM {tableName}", connection);
                var reader = command.ExecuteReader();
                var idsToDelete = new List<int>();

                while (reader.Read())
                {
                    var id = reader.GetInt32(0);
                    var json = reader.GetString(1);
                    var item = JsonConvert.DeserializeObject<T>(json);
                    if (predicate.Compile()(item))
                    {
                        idsToDelete.Add(id);
                    }
                }

                reader.Close();

                foreach (var id in idsToDelete)
                {
                    var deleteCommand = new SQLiteCommand($"DELETE FROM {tableName} WHERE Id = @Id", connection);
                    deleteCommand.Parameters.AddWithValue("@Id", id);
                    deleteCommand.ExecuteNonQuery();
                }
            }
        }

        public void Update<T>(string tableName, Expression<Func<T, bool>> predicate, T value)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                var command = new SQLiteCommand($"SELECT Id, JsonData FROM {tableName}", connection);
                var reader = command.ExecuteReader();
                int idToUpdate = -1;

                while (reader.Read())
                {
                    var id = reader.GetInt32(0);
                    var json = reader.GetString(1);
                    var item = JsonConvert.DeserializeObject<T>(json);
                    if (predicate.Compile()(item))
                    {
                        idToUpdate = id;
                        break;
                    }
                }

                reader.Close();

                if (idToUpdate >= 0)
                {
                    var json = JsonConvert.SerializeObject(value);
                    var updateCommand = new SQLiteCommand($"UPDATE {tableName} SET JsonData = @JsonData WHERE Id = @Id", connection);
                    updateCommand.Parameters.AddWithValue("@JsonData", json);
                    updateCommand.Parameters.AddWithValue("@Id", idToUpdate);
                    updateCommand.ExecuteNonQuery();
                }
            }
        }

        public void DeleteAll(string tableName)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                var command = new SQLiteCommand($"DELETE FROM {tableName}", connection);
                command.ExecuteNonQuery();
            }
        }
    }
}
