using DevFlow.Api.DTOs;

namespace DevFlow.Api.Services;

public interface IProductService
{
    Task<ProductResponse> CreateAsync(CreateProductRequest request);
    Task<ProductResponse> UpdateAsync(Guid id, UpdateProductRequest request);
    Task<ProductResponse> ArchiveAsync(Guid id);
    Task<ProductResponse> GetByIdAsync(Guid id);
    Task<IReadOnlyList<ProductResponse>> ListAsync();
}
