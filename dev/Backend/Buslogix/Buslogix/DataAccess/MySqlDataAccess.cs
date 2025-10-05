using System.Data;
using Buslogix.Interfaces;
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
