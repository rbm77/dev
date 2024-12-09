using System.Data;
using System.Data.Common;

namespace Pos.Interfaces
{
    public interface IDatabase
    {
        Task<T> ExecuteReader<T>(string procedureName, Func<IDataReader, T> map, IEnumerable<DbParameter> parameters);

        Task<int> ExecuteNonQuery(string procedureName, IEnumerable<DbParameter> parameters);

        Task<object?> ExecuteScalar(string procedureName, IEnumerable<DbParameter> parameters);

        DbParameter CreateParameter(string name, object? value);
    }
}
