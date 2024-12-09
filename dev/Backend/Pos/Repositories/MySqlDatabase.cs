using System.Data;
using System.Data.Common;
using MySqlConnector;
using Pos.Interfaces;

namespace Pos.Repositories
{
    public class MySqlDatabase(string connectionString) : IDatabase
    {

        public DbParameter CreateParameter(string name, object value)
        {
            return new MySqlParameter(name, value);
        }

        public async Task<int> ExecuteNonQuery(string procedureName, IEnumerable<DbParameter> parameters)
        {
            await using MySqlConnection connection = new(connectionString);
            await connection.OpenAsync();

            await using MySqlCommand command = new(procedureName, connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            foreach (DbParameter parameter in parameters)
            {
                _ = command.Parameters.Add(parameter);
            }

            return await command.ExecuteNonQueryAsync();
        }

        public async Task<T> ExecuteReader<T>(string procedureName, Func<IDataReader, T> map, IEnumerable<DbParameter> parameters)
        {
            await using MySqlConnection connection = new(connectionString);
            await connection.OpenAsync();

            await using MySqlCommand command = new(procedureName, connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            foreach (DbParameter parameter in parameters)
            {
                _ = command.Parameters.Add(parameter);
            }

            await using MySqlDataReader reader = await command.ExecuteReaderAsync();
            return map(reader);
        }


        public async Task<object?> ExecuteScalar(string procedureName, IEnumerable<DbParameter> parameters)
        {
            await using MySqlConnection connection = new(connectionString);
            await connection.OpenAsync();

            await using MySqlCommand command = new(procedureName, connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            foreach (DbParameter parameter in parameters)
            {
                _ = command.Parameters.Add(parameter);
            }

            return await command.ExecuteScalarAsync();
        }
    }
}
