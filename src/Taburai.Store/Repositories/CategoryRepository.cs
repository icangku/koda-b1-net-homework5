using Microsoft.Data.SqlClient;
namespace Taburai.Store.Repositories;

public interface ICategoryRepository
{
  Task<List<Category>> GetAllAsync();
  Task<Category?> GetByIdAsync(int id);
  Task<bool> NameExistsAsync(string name, int exceptId = 0);
  Task<bool> HasFoodsAsync(int id);
  Task<int> CreateAsync(string name);
  Task<bool> UpdateAsync(int id, string name);
  Task<bool> DeleteAsync(int id);
}

public sealed class CategoryRepository(IDbConnectionFactory db) : ICategoryRepository
{
  private const string Cols = "Id, Name, CreatedAt, UpdatedAt";

  private static Category Map(SqlDataReader r) => new(r.GetInt32(0), r.GetString(1), r.GetDateTime(2), r.IsDBNull(3) ? null : r.GetDateTime(3));

  public async Task<List<Category>> GetAllAsync()
  {
    await using var conn = await db.OpenAsync();
    await using var cmd = new SqlCommand($"SELECT {Cols} FROM Category ORDER BY Name", conn);
    await using var r = await cmd.ExecuteReaderAsync();
    var list = new List<Category>();
    while (await r.ReadAsync()) list.Add(Map(r));
    return list;
  }

  public async Task<Category?> GetByIdAsync(int id)
  {
    await using var conn = await db.OpenAsync();
    await using var cmd = new SqlCommand($"SELECT {Cols} FROM Category WHERE Id = @id", conn).Int("@id", id);
    await using var r = await cmd.ExecuteReaderAsync();
    return await r.ReadAsync() ? Map(r) : null;
  }

  public async Task<bool> NameExistsAsync(string name, int exceptId = 0)
  {
    await using var conn = await db.OpenAsync();
    await using var cmd = new SqlCommand("SELECT 1 FROM Category WHERE Name = @name AND Id <> @exceptId", conn)
      .Str("@name", name, 100).Int("@exceptId", exceptId);
    return await cmd.ExecuteScalarAsync() is not null;
  }

  public async Task<bool> HasFoodsAsync(int id)
  {
    await using var conn = await db.OpenAsync();
    await using var cmd = new SqlCommand("SELECT 1 FROM Food WHERE CategoryId = @id", conn).Int("@id", id);
    return await cmd.ExecuteScalarAsync() is not null;
  }

  public async Task<int> CreateAsync(string name)
  {
    await using var conn = await db.OpenAsync();
    await using var cmd = new SqlCommand("INSERT INTO Category (Name) OUTPUT INSERTED.Id VALUES (@name)", conn)
      .Str("@name", name, 100);
    return (int)(await cmd.ExecuteScalarAsync())!;
  }

  public async Task<bool> UpdateAsync(int id, string name)
  {
    await using var conn = await db.OpenAsync();
    await using var cmd = new SqlCommand("UPDATE Category SET Name = @name, UpdatedAt = SYSDATETIME() WHERE Id = @id", conn)
      .Str("@name", name, 100).Int("@id", id);
    return await cmd.ExecuteNonQueryAsync() > 0;
  }

  public async Task<bool> DeleteAsync(int id)
  {
    await using var conn = await db.OpenAsync();
    await using var cmd = new SqlCommand("DELETE FROM Category WHERE Id = @id", conn).Int("@id", id);
    return await cmd.ExecuteNonQueryAsync() > 0;
  }
}