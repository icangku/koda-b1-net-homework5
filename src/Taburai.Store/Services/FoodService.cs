namespace Taburai.Store.Services;

public interface IFoodService
{
  Task<List<FoodResponse>> GetAllAsync(int? categoryId);
  Task<FoodResponse> GetByIdAsync(int id);
  Task<FoodResponse> CreateAsync(FoodRequest req);
  Task<FoodResponse> UpdateAsync(int id, FoodRequest req);
  Task DeleteAsync(int id);
}

public sealed class FoodService(IFoodRepository foods, ICategoryRepository categories) : IFoodService
{
  private static FoodResponse Map(FoodDetail f) =>
    new(f.Id, f.Name, f.Price, f.Stock, f.CategoryId, f.CategoryName, f.CreatedAt, f.UpdatedAt);

  private static string Clean(string name)
  {
    var n = name.Trim();
    return n.Length == 0 ? throw new BadRequestException("Name is required") : n;
  }

  public async Task<List<FoodResponse>> GetAllAsync(int? categoryId) =>
    (await foods.GetAllAsync(categoryId)).Select(Map).ToList();

  public async Task<FoodResponse> GetByIdAsync(int id) =>
    Map(await foods.GetByIdAsync(id) ?? throw new NotFoundException("Food not found"));

  public async Task<FoodResponse> CreateAsync(FoodRequest req)
  {
    var name = Clean(req.Name);
    if (await categories.GetByIdAsync(req.CategoryId) is null) throw new NotFoundException("Category not found");
    if (await foods.NameExistsAsync(name)) throw new ConflictException("Food name already exists");
    return await GetByIdAsync(await foods.CreateAsync(name, req.Price, req.Stock, req.CategoryId));
  }

  public async Task<FoodResponse> UpdateAsync(int id, FoodRequest req)
  {
    var name = Clean(req.Name);
    if (await foods.GetByIdAsync(id) is null) throw new NotFoundException("Food not found");
    if (await categories.GetByIdAsync(req.CategoryId) is null) throw new NotFoundException("Category not found");
    if (await foods.NameExistsAsync(name, id)) throw new ConflictException("Food name already exists");
    await foods.UpdateAsync(id, name, req.Price, req.Stock, req.CategoryId);
    return await GetByIdAsync(id);
  }

  public async Task DeleteAsync(int id)
  {
    if (await foods.GetByIdAsync(id) is null) throw new NotFoundException("Food not found");
    await foods.DeleteAsync(id); // sudah pernah dipesan -> FK menolak -> handler jadi 409
  }
}