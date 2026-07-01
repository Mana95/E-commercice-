namespace DevFlow.Api.Services;

public class DuplicateSkuException : Exception
{
    public DuplicateSkuException() : base("A product with this SKU already exists.")
    {
    }
}

public class InvalidProductException : Exception
{
    public InvalidProductException(string message) : base(message)
    {
    }
}

public class ProductNotFoundException : Exception
{
    public ProductNotFoundException() : base("Product not found.")
    {
    }
}

public class CategoryNotFoundException : Exception
{
    public CategoryNotFoundException() : base("Category not found.")
    {
    }
}
