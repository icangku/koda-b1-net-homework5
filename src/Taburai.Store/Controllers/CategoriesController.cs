using Microsoft.AspNetCore.Mvc;
namespace Taburai.Store.Controllers;

[ApiController]
[Route("api/categories")]
public class CategoriesController(ICategoryService service) : ControllerBase
{
  [HttpGet]
  public async Task<ActionResult<List<CategoryResponse>>> GetAll() => await service.GetAllAsync();

  [HttpGet("{id:int}")]
  public async Task<ActionResult<CategoryResponse>> GetById(int id) => await service.GetByIdAsync(id);

  [HttpPost]
  public async Task<ActionResult<CategoryResponse>> Create(CategoryRequest req)
  {
    var created = await service.CreateAsync(req);
    return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
  }

  [HttpPut("{id:int}")]
  public async Task<ActionResult<CategoryResponse>> Update(int id, CategoryRequest req) =>
    await service.UpdateAsync(id, req);

  [HttpDelete("{id:int}")]
  public async Task<IActionResult> Delete(int id)
  {
    await service.DeleteAsync(id);
    return NoContent();
  }
}