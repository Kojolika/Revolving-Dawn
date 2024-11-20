using System;
using Mono.Data.Sqlite;
using Tooling.Logging;
using UnityEngine;

namespace Data.DB
{
    /// <summary>
    /// Following the docs: https://www.mono-project.com/docs/database-access/providers/sqlite/
    /// </summary>
    public class DBInterface : IDisposable
    {
        /// <summary>
        /// Use a single file as our database.
        /// </summary>
        private static readonly string ConnectionString = $"Data Source=file:{DBName},version=3";
        private const string DBName = "revolving.db";
        public static readonly string DBFilePath = $"{Application.persistentDataPath}/${DBName}";

        private SqliteCommand command;
        private SqliteConnection connection;

        public void OpenConnection()
        {
            connection = new SqliteConnection(ConnectionString);
            connection.Open();
            MyLogger.Log($"Opened database: {connection.Database}");
            command = connection.CreateCommand();
        }

        public void Dispose()
        {
            command?.Dispose();
            connection?.Close();
        }
    }
}