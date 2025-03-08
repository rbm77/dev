using System.Data;
using Buslogix.Interfaces;
using MySqlConnector;

namespace Buslogix.DataAccess
{
    public class MySqlDataAccess : IDataAccess
    {
        private readonly string _connectionString;

        public MySqlDataAccess(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Invalid connection string");
            }
            _connectionString = connectionString;
        }

        public async Task<int> ExecuteNonQuery(string commandText, CommandType commandType, IDataParameter[] parameters)
        {
            await using MySqlConnection connection = new(_connectionString);
            await connection.OpenAsync();

            await using MySqlCommand command = new(commandText, connection)
            {
                CommandType = commandType
            };

            if (parameters != null && parameters.Length > 0)
            {
                command.Parameters.AddRange(parameters);
            }

            return await command.ExecuteNonQueryAsync();
        }

        public async Task<List<T>> ExecuteReader<T>(string commandText, CommandType commandType, Func<IDataReader, T> map, IDictionary<string, object> parameters)
        {
            List<T> results = [];

            await using MySqlConnection connection = new(_connectionString);
            await connection.OpenAsync();

            await using MySqlCommand command = new(commandText, connection)
            {
                CommandType = commandType
            };

            if (parameters != null && parameters.Count > 0)
            {
                command.Parameters.AddRange(parameters.Select(static p => new MySqlParameter(p.Key, p.Value)).ToArray());
            }

            await using MySqlDataReader reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                results.Add(map(reader));
            }

            return results;
        }

        public async Task<object?> ExecuteScalar(string commandText, CommandType commandType, IDataParameter[] parameters)
        {
            await using MySqlConnection connection = new(_connectionString);
            await connection.OpenAsync();

            await using MySqlCommand command = new(commandText, connection)
            {
                CommandType = commandType
            };

            if (parameters != null && parameters.Length > 0)
            {
                command.Parameters.AddRange(parameters);
            }

            return await command.ExecuteScalarAsync();
        }
    }
}
