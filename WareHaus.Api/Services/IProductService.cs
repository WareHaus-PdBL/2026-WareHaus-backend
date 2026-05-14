using WareHaus.Api.DTOs;

namespace WareHaus.Api.Services;

public interface IProductService
{
    Task<List<GetProductDto>> GetAllProductsAsync();

    Task<GetProductDetailDto> GetProductDetailsAsync(int productId);

    Task<GetProductDto> CreateProductAsync(CreateProductDto createProductDto);

    Task<GetProductDto> UpdateProductAsync(int productId, UpdateProductDto updateProductDto);

    Task DeleteProductAsync(int productId);

    Task<List<GetProductStockLocationDto>> GetProductStockLocationsAsync(int productId);

    Task<GetProductDetailDto> AddProductStockLocationAsync(
        AddProductStockLocationDto addStockLocationDto
    );

    Task<GetProductDetailDto> UpdateProductStockLocationAsync(
        UpdateProductStockLocationDto updateStockLocationDto
    );

    Task DeleteProductStockLocationAsync(int productId, int shelfId);
}