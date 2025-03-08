using System.Data;

namespace Buslogix.Interfaces
{
    public interface IDataAccess
    {
        Task<int> ExecuteNonQuery(string commandText, CommandType commandType, IDataParameter[] parameters);
        Task<object?> ExecuteScalar(string commandText, CommandType commandType, IDataParameter[] parameters);
        Task<List<T>> ExecuteReader<T>(string commandText, CommandType commandType, Func<IDataReader, T> map, IDictionary<string, object> parameters);
    }
}
