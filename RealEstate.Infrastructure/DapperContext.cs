using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using RealEstate.Application.Interfaces;

public class DapperContext : IDapperContext
{
    private readonly IDbConnection _db;
    public DapperContext(IDbConnection db) => _db = db;

    public Task<int> ExecuteAsync(string sql, object param = null, CommandType? commandType = null) =>
        _db.ExecuteAsync(sql, param, commandType: commandType);

    public Task<IEnumerable<T>> QueryAsync<T>(string sql, object param = null, CommandType? commandType = null) =>
        _db.QueryAsync<T>(sql, param, commandType: commandType);
}
