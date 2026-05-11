namespace SOSApp.Contract;

public class ProductDto
{
    public int Id { get; init; }
    public required string Name { get; init; }
    public required decimal Price { get; init; }
    public required ProductCategory Category { get; init; }
}

