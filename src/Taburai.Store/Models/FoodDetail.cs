namespace Taburai.Store.Models;

public sealed class FoodDetail(int id, string name, decimal price, int stock,
                               int categoryId, string categoryName,
                               DateTime createdAt, DateTime? updatedAt)
{
  public int Id {get;} = id;
  public string Name {get;} = name;
  public decimal Price {get;} = price;
  public int Stock {get;} = stock;
  public int CategoryId {get;} = categoryId;
  public string CategoryName {get;} = categoryName;
  public DateTime CreatedAt {get;} = createdAt;
  public DateTime? UpdatedAt {get;} = updatedAt;
}