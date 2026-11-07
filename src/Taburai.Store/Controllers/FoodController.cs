using Microsoft.AspNetCore.Mvc;
namespace Taburai.Store.Controllers;

[ApiController]
[Route("api/foods")]
public class FoodsController(IFoodService service) : ControllerBase
{
  [HttpGet]
  public async Task<ActionResult<List<FoodResponse>>> GetAll([FromQuery] int? categoryId) =>
    await service.GetAllAsync(categoryId);

  [HttpGet("{id:int}")]
  public async Task<ActionResult<FoodResponse>> GetById(int id) => await service.GetByIdAsync(id);

  [HttpPost]
  public async Task<ActionResult<FoodResponse>> Create(FoodRequest req)
  {
    var created = await service.CreateAsync(req);
    return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
  }

  [HttpPut("{id:int}")]
  public async Task<ActionResult<FoodResponse>> Update(int id, FoodRequest req) =>
    await service.UpdateAsync(id, req);

  [HttpDelete("{id:int}")]
  public async Task<IActionResult> Delete(int id)
  {
    await service.DeleteAsync(id);
    return NoContent();
  }
}