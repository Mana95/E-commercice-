using DevFlow.Api.Models;

namespace DevFlow.Api.DTOs;

public class UpdateProductRequest
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public Guid CategoryId { get; set; }
    public Guid? BrandId { get; set; }
    public ProductStatus Status { get; set; }
}
