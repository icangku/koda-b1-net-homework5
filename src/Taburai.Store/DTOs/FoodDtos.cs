using System.ComponentModel.DataAnnotations;

namespace Taburai.Store.DTOs;

public class FoodRequest
{
  [Required]
  [StringLength(100, MinimumLength = 1)]
  public string Name {get; set;} = null!;

  [Range(typeof(decimal), "0", "999999999999999")]
  public decimal Price {get; set;}

  [Range(0, int.MaxValue)]
  public int Stock {get; set;}

  [Range(1, int.MaxValue)]
  public int CategoryId {get; set;}
}
public record FoodResponse(int Id, string Name, decimal Price, int Stock,
                           int CategoryId, string CategoryName,
                           DateTime CreatedAt, DateTime? UpdatedAt);