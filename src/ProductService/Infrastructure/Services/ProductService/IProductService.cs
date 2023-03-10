using Domain.Entities;
using Domain.Primitives;
using Infrastructure.ListQuery;
using Infrastructure.SortingSystem;

namespace Infrastructure.Services.ProductService;

public interface IProductService
{
    public Task<Product> GetProductById(Guid productId);
    public Task<Product> GetProductByName(NotNullString productName);
    public Task<List<Product>> GetProducts(Pagination pagination, ProductSortingInfo productSortingInfo);
    public Task<Product> CreateNewProduct(CreateProductCommand createProductCommand);
}