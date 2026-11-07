namespace Taburai.Store.DTOs;

using System.ComponentModel.DataAnnotations;

public class CategoryRequest
{
  [Required]
  [StringLength(100, MinimumLength = 1)]
  public string Name {get; set;} = null!;
}

public record CategoryResponse(int Id, string Name, DateTime CreatedAt, DateTime? UpdatedAt);