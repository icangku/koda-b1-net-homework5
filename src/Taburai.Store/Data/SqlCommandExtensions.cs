using System.Data;
using Microsoft.Data.SqlClient;
namespace Taburai.Store.Data;

public static class SqlCommandExtensions
{
  public static SqlCommand Int(this SqlCommand c, string name, int value)
  {
    c.Parameters.Add(name, SqlDbType.Int).Value = value;
    return c;
  }

  public static SqlCommand Str(this SqlCommand c, string name, string value, int size)
  {
    c.Parameters.Add(name, SqlDbType.NVarChar, size).Value = value;
    return c;
  }

  public static SqlCommand Dec(this SqlCommand c, string name, decimal value)
  {
    var p = c.Parameters.Add(name, SqlDbType.Decimal);
    p.Precision = 19;
    p.Scale = 4;
    p.Value = value;
    return c;
  }
}