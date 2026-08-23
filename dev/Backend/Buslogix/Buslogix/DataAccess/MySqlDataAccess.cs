using System.Data;
using Buslogix.Interfaces;
using Buslogix.Utilities;
using MySqlConnector;

namespace Buslogix.DataAccess
{
    public class MySqlDataAccess(string connectionString) : IDataAccess
    {

        public async Task<int> ExecuteNonQuery(string commandText, CommandType commandType, IDictionary<string, object?>? parameters)
        {
            await using MySqlConnection connection = new(connectionString);
            await connection.OpenAsync();

            await using MySqlCommand command = new(commandText, connection)
            {
                CommandType = commandType
            };

            if (parameters != null && parameters.Count > 0)
            {
                command.Parameters.AddRange(parameters.Select(static p => new MySqlParameter(p.Key, p.Value ?? DBNull.Value)).ToArray());
            }

            return await command.ExecuteNonQueryAsync();
        }

        public async Task<(int AffectedRows, IDictionary<string, object?> OutputValues)> ExecuteNonQuery(string commandText, CommandType commandType, IDictionary<string, object?>? parameters, IDictionary<string, DbType> outputParameters)
        {
            await using MySqlConnection connection = new(connectionString);
            await connection.OpenAsync();

            await using MySqlCommand command = new(commandText, connection)
            {
                CommandType = commandType
            };

            if (parameters != null && parameters.Count > 0)
            {
                command.Parameters.AddRange(parameters.Select(static p => new MySqlParameter(p.Key, p.Value ?? DBNull.Value)).ToArray());
            }

            foreach (KeyValuePair<string, DbType> output in outputParameters)
            {
                command.Parameters.Add(new MySqlParameter(output.Key, null)
                {
                    DbType = output.Value,
                    Direction = ParameterDirection.Output
                });
            }

            int affected = await command.ExecuteNonQueryAsync();

            Dictionary<string, object?> outputValues = new();
            foreach (string name in outputParameters.Keys)
            {
                object? value = command.Parameters[name].Value;
                outputValues[name] = value is null or DBNull ? null : value;
            }

            return (affected, outputValues);
        }

        public async Task<List<T>> ExecuteReader<T>(string commandText, CommandType commandType, Func<IDataReader, T> map, IDictionary<string, object?>? parameters)
        {
            List<T> results = [];

            await using MySqlConnection connection = new(connectionString);
            await connection.OpenAsync();

            await using MySqlCommand command = new(commandText, connection)
            {
                CommandType = commandType
            };

            if (parameters != null && parameters.Count > 0)
            {
                command.Parameters.AddRange(parameters.Select(static p => new MySqlParameter(p.Key, p.Value ?? DBNull.Value)).ToArray());
            }

            await using MySqlDataReader reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                results.Add(map(reader));
            }

            return results;
        }

        public async Task<(List<T> Items, long TotalCount)> ExecuteReaderPaged<T>(string commandText, CommandType commandType, Func<IDataReader, T> map, IDictionary<string, object?>? parameters)
        {
            List<T> results = [];
            long totalCount = 0;

            await using MySqlConnection connection = new(connectionString);
            await connection.OpenAsync();

            await using MySqlCommand command = new(commandText, connection)
            {
                CommandType = commandType
            };

            if (parameters != null && parameters.Count > 0)
            {
                command.Parameters.AddRange(parameters.Select(static p => new MySqlParameter(p.Key, p.Value ?? DBNull.Value)).ToArray());
            }

            await using MySqlDataReader reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                results.Add(map(reader));
            }

            if (await reader.NextResultAsync() && await reader.ReadAsync())
            {
                totalCount = reader.GetInt64OrDefault(0);
            }

            return (results, totalCount);
        }

        public async Task<object?> ExecuteScalar(string commandText, CommandType commandType, IDictionary<string, object?>? parameters)
        {
            await using MySqlConnection connection = new(connectionString);
            await connection.OpenAsync();

            await using MySqlCommand command = new(commandText, connection)
            {
                CommandType = commandType
            };

            if (parameters != null && parameters.Count > 0)
            {
                command.Parameters.AddRange(parameters.Select(static p => new MySqlParameter(p.Key, p.Value ?? DBNull.Value)).ToArray());
            }

            return await command.ExecuteScalarAsync();
        }
    }
}
