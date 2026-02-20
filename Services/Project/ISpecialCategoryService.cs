using MoneyMood.Dtos.Category;

namespace MoneyMood.Services.Project;

public interface ISpecialCategoryService
{
    Task<CategoryResponse> CreateSpecialCategoryAsync(Guid userId, Guid budgetId, CategoryRequest request);
    Task<CategoryResponse> UpdateSpecialCategoryAsync(Guid userId, Guid budgetId, Guid categoryId, CategoryRequest request);
    Task<DeletedCategoryResponse> DeleteSpecialCategoryAsync(Guid userId, Guid budgetId, Guid categoryId);
    Task<DeletedCategoryCascadeResponse> DeleteSpecialCategoryOnCascadeAsync(Guid userId, Guid budgetId, Guid categoryId);
    
}