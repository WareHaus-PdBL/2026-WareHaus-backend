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
    public async Task<ActionResult<GetProductDto>> CreateProductAsync(CreateProductDto createProductDto)
    {
        var product = await _productService.CreateProductAsync(createProductDto);
        return Ok(product);
    }

    [HttpPut("{productId}")]
    public async Task<ActionResult<GetProductDto>> UpdateProductAsync(int productId, UpdateProductDto updateProductDto)
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
}