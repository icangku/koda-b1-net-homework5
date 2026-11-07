using Microsoft.Data.SqlClient;
namespace Taburai.Store.Data;

public interface IDbConnectionFactory
{
  Task<SqlConnection> OpenAsync();
}

public sealed class SqlConnectionFactory(IConfiguration config) : IDbConnectionFactory
{
  private readonly string _cs = config.GetConnectionString("Default")
    ?? throw new InvalidOperationException("ConnectionStrings:Default is missing");

  public async Task<SqlConnection> OpenAsync()
  {
    var conn = new SqlConnection(_cs);
    await conn.OpenAsync();
    return conn;
  }
}