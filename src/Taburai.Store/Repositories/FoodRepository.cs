using Microsoft.Data.SqlClient;
namespace Taburai.Store.Repositories;

public interface IFoodRepository
{
  Task<List<FoodDetail>> GetAllAsync(int? categoryId);
  Task<FoodDetail?> GetByIdAsync(int id);
  Task<bool> NameExistsAsync(string name, int exceptId = 0);
  Task<int> CreateAsync(string name, decimal price, int stock, int categoryId);
  Task<bool> UpdateAsync(int id, string name, decimal price, int stock, int categoryId);
  Task<bool> DeleteAsync(int id);
}

public sealed class FoodRepository(IDbConnectionFactory db) : IFoodRepository
{
  private const string Select =
    @"SELECT f.Id, f.Name, f.Price, f.Stock, f.CategoryId, c.Name, f.CreatedAt, f.UpdatedAt
      FROM Food f
      INNER JOIN Category c ON c.Id = f.CategoryId";

  private static FoodDetail Map(SqlDataReader r) =>
    new(r.GetInt32(0), r.GetString(1), r.GetDecimal(2), r.GetInt32(3),
        r.GetInt32(4), r.GetString(5), r.GetDateTime(6), r.IsDBNull(7) ? null : r.GetDateTime(7));

  public async Task<List<FoodDetail>> GetAllAsync(int? categoryId)
  {
    await using var conn = await db.OpenAsync();
    var sql = Select + (categoryId is null ? "" : " WHERE f.CategoryId = @categoryId") + " ORDER BY f.Name";
    await using var cmd = new SqlCommand(sql, conn);
    if (categoryId is not null) cmd.Int("@categoryId", categoryId.Value);
    await using var r = await cmd.ExecuteReaderAsync();
    var list = new List<FoodDetail>();
    while (await r.ReadAsync()) list.Add(Map(r));
    return list;
  }

  public async Task<FoodDetail?> GetByIdAsync(int id)
  {
    await using var conn = await db.OpenAsync();
    await using var cmd = new SqlCommand(Select + " WHERE f.Id = @id", conn).Int("@id", id);
    await using var r = await cmd.ExecuteReaderAsync();
    return await r.ReadAsync() ? Map(r) : null;
  }

  public async Task<bool> NameExistsAsync(string name, int exceptId = 0)
  {
    await using var conn = await db.OpenAsync();
    await using var cmd = new SqlCommand("SELECT 1 FROM Food WHERE Name = @name AND Id <> @exceptId", conn)
      .Str("@name", name, 100).Int("@exceptId", exceptId);
    return await cmd.ExecuteScalarAsync() is not null;
  }

  public async Task<int> CreateAsync(string name, decimal price, int stock, int categoryId)
  {
    await using var conn = await db.OpenAsync();
    await using var cmd = new SqlCommand(
      "INSERT INTO Food (Name, Price, Stock, CategoryId) OUTPUT INSERTED.Id VALUES (@name, @price, @stock, @categoryId)", conn)
      .Str("@name", name, 100).Dec("@price", price).Int("@stock", stock).Int("@categoryId", categoryId);
    return (int)(await cmd.ExecuteScalarAsync())!;
  }

  public async Task<bool> UpdateAsync(int id, string name, decimal price, int stock, int categoryId)
  {
    await using var conn = await db.OpenAsync();
    await using var cmd = new SqlCommand(
      @"UPDATE Food
        SET Name = @name, Price = @price, Stock = @stock, CategoryId = @categoryId, UpdatedAt = SYSDATETIME()
        WHERE Id = @id", conn)
      .Str("@name", name, 100).Dec("@price", price).Int("@stock", stock)
      .Int("@categoryId", categoryId).Int("@id", id);
    return await cmd.ExecuteNonQueryAsync() > 0;
  }

  public async Task<bool> DeleteAsync(int id)
  {
    await using var conn = await db.OpenAsync();
    await using var cmd = new SqlCommand("DELETE FROM Food WHERE Id = @id", conn).Int("@id", id);
    return await cmd.ExecuteNonQueryAsync() > 0;
  }
}