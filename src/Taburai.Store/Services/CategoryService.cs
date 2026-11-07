namespace Taburai.Store.Services;

public interface ICategoryService
{
  Task<List<CategoryResponse>> GetAllAsync();
  Task<CategoryResponse> GetByIdAsync(int id);
  Task<CategoryResponse> CreateAsync(CategoryRequest req);
  Task<CategoryResponse> UpdateAsync(int id, CategoryRequest req);
  Task DeleteAsync(int id);
}

public sealed class CategoryService(ICategoryRepository repo) : ICategoryService
{
  private static CategoryResponse Map(Category c) => new(c.Id, c.Name, c.CreatedAt, c.UpdatedAt);

  private static string Clean(string name)
  {
    var n = name.Trim();
    return n.Length == 0 ? throw new BadRequestException("Name is required") : n;
  }

  public async Task<List<CategoryResponse>> GetAllAsync() =>
    (await repo.GetAllAsync()).Select(Map).ToList();

  public async Task<CategoryResponse> GetByIdAsync(int id) =>
    Map(await repo.GetByIdAsync(id) ?? throw new NotFoundException("Category not found"));

  public async Task<CategoryResponse> CreateAsync(CategoryRequest req)
  {
    var name = Clean(req.Name);
    if (await repo.NameExistsAsync(name)) throw new ConflictException("Category name already exists");
    return await GetByIdAsync(await repo.CreateAsync(name));
  }

  public async Task<CategoryResponse> UpdateAsync(int id, CategoryRequest req)
  {
    var name = Clean(req.Name);
    if (await repo.GetByIdAsync(id) is null) throw new NotFoundException("Category not found");
    if (await repo.NameExistsAsync(name, id)) throw new ConflictException("Category name already exists");
    await repo.UpdateAsync(id, name);
    return await GetByIdAsync(id);
  }

  public async Task DeleteAsync(int id)
  {
    if (await repo.GetByIdAsync(id) is null) throw new NotFoundException("Category not found");
    if (await repo.HasFoodsAsync(id)) throw new ConflictException("Category still has foods");
    await repo.DeleteAsync(id);
  }
}