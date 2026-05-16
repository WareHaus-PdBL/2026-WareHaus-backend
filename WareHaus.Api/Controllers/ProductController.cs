using Microsoft.AspNetCore.Mvc;
using WareHaus.Api.DTOs;
using WareHaus.Api.Services;

namespace WareHaus.Api.Controllers;

[ApiController]
[Route("v1/[controller]")]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<ActionResult<List<GetProductDto>>> GetAllProductsAsync()
    {
        var products = await _productService.GetAllProductsAsync();
        return Ok(products);
    }

    [HttpGet("{productId}")]
    public async Task<ActionResult<GetProductDetailDto>> GetProductDetailsAsync(int productId)
    {
        var product = await _productService.GetProductDetailsAsync(productId);
        return Ok(product);
    }

    [HttpPost]
    public async Task<ActionResult<GetProductDto>> CreateProductAsync(
        [FromBody] CreateProductDto createProductDto
    )
    {
        var product = await _productService.CreateProductAsync(createProductDto);
        return Ok(product);
    }

    [HttpPut("{productId}")]
    public async Task<ActionResult<GetProductDto>> UpdateProductAsync(
        int productId,
        [FromBody] UpdateProductDto updateProductDto
    )
    {
        var product = await _productService.UpdateProductAsync(productId, updateProductDto);
        return Ok(product);
    }

    [HttpDelete("{productId}")]
    public async Task<IActionResult> DeleteProductAsync(int productId)
    {
        await _productService.DeleteProductAsync(productId);
        return NoContent();
    }

    [HttpGet("{productId}/stock-locations")]
    public async Task<ActionResult<List<GetProductStockLocationDto>>> GetProductStockLocationsAsync(
        int productId
    )
    {
        var stockLocations = await _productService.GetProductStockLocationsAsync(productId);
        return Ok(stockLocations);
    }

    [HttpPost("stock-locations")]
    public async Task<ActionResult<GetProductDetailDto>> AddProductStockLocationAsync(
        [FromBody] AddProductStockLocationDto addStockLocationDto
    )
    {
        var product = await _productService.AddProductStockLocationAsync(
            addStockLocationDto
        );

        return Ok(product);
    }

    [HttpPut("stock-locations")]
    public async Task<ActionResult<GetProductDetailDto>> UpdateProductStockLocationAsync(
        [FromBody] UpdateProductStockLocationDto updateStockLocationDto
    )
    {
        var product = await _productService.UpdateProductStockLocationAsync(
            updateStockLocationDto
        );

        return Ok(product);
    }

    [HttpPost("stock-locations/move")]
    public async Task<ActionResult<GetProductDetailDto>> MoveProductStockLocationAsync(
        [FromBody] MoveProductStockLocationDto moveStockLocationDto
    )
    {
        var product = await _productService.MoveProductStockLocationAsync(
            moveStockLocationDto
        );

        return Ok(product);
    }

    [HttpDelete("{productId}/stock-locations/{shelfId}")]
    public async Task<IActionResult> DeleteProductStockLocationAsync(
        int productId,
        int shelfId
    )
    {
        await _productService.DeleteProductStockLocationAsync(productId, shelfId);
        return NoContent();
    }
}