using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace RealEstate.Application.Interfaces
{
    public interface IDapperContext
    {
        Task<int> ExecuteAsync(string sql, object param = null, CommandType? commandType = null);
        Task<IEnumerable<T>> QueryAsync<T>(string sql, object param = null, CommandType? commandType = null);
    }
}