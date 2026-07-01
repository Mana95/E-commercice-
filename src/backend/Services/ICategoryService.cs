using DevFlow.Api.DTOs;

namespace DevFlow.Api.Services;

public interface ICategoryService
{
    Task<CategoryResponse> CreateAsync(CreateCategoryRequest request);
    Task<IReadOnlyList<CategoryResponse>> ListAsync();
}
