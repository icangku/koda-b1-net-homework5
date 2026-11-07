using System.ComponentModel.DataAnnotations;

namespace Taburai.Store.DTOs;

public class CartItemRequest
{
  [Range(1, int.MaxValue)]
  public int FoodId {get; set;}

  [Range(1, int.MaxValue)]
  public int Qty {get; set;}
}

public record CartItemResponse(int Id, int FoodId, string FoodName, decimal UnitPrice, int Qty, decimal Subtotal);

public record CartResponse(int Id, List<CartItemResponse> Items, decimal Total, DateTime CreatedAt, DateTime? UpdatedAt);