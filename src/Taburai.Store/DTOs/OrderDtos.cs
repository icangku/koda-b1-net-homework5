using System.ComponentModel.DataAnnotations;

namespace Taburai.Store.DTOs;

public class OrderItemRequest
{
  [Range(1, int.MaxValue)]
  public int FoodId {get; set;}

  [Range(1, int.MaxValue)]
  public int Qty {get; set;}
}

public class CreateOrderRequest
{
  [Range(1, int.MaxValue)]
  public int PaymentMethodId {get; set;}

  [Required]
  [MinLength(1)]
  public List<OrderItemRequest> Items {get; set;} = new();
}

public record OrderItemResponse(int Id, int FoodId, string FoodName, int Qty, decimal UnitPrice, decimal Subtotal);

public record OrderResponse(int Id, int PaymentMethodId, string PaymentMethodName,
                            decimal TotalAmount, List<OrderItemResponse> Items,
                            DateTime CreatedAt, DateTime? UpdatedAt);