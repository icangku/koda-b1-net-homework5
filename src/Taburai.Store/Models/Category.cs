namespace Taburai.Store.Models;

public sealed class Category(int id, string name, DateTime createdAt, DateTime? updatedAt)
{
  public int Id {get;} = id;
  public string Name {get; init;} = name;
  public DateTime CreatedAt {get;} = createdAt;
  public DateTime? UpdatedAt {get;} = updatedAt;
}