using WareHaus.Api.Data;
using WareHaus.Api.DTOs;
using Microsoft.EntityFrameworkCore;

namespace WareHaus.Api.Services;

class ProductService : IProductService
{
    private readonly AppDbContext _context;

    public ProductService(AppDbContext context)
    {
        _context = context;
    }

    // Get All Products
    public async Task<List<GetProductDto>> GetAllProductsAsync()
    {
        return await _context.Products
            .Select(p => new GetProductDto(
                p.Id,
                p.SKU,
                p.ProductName,
                p.Barcode,
                p.UnitOfMeasure))
            .ToListAsync();
    }

    // Get Product Details by Id
    public async Task<GetProductDetailDto> GetProductDetailsAsync(int productId)
    {
        var product = await _context.Products.Where(p => p.Id == productId)
            .Select(p => new GetProductDetailDto(
                p.Id,
                p.SKU,
                p.ProductName,
                p.Barcode,
                p.UnitOfMeasure,
                p.Stocks.Select(s => new GetStockDto(
                    s.Id,
                    s.ShelfId,
                    s.ProductId,
                    s.Quantity,
                    new GetShelfForZoneDto(
                        s.Shelf.Id,
                        s.Shelf.ShelfCode,
                        s.Shelf.Aisle,
                        s.Shelf.Capacity,
                        s.Shelf.CurrentVolume,
                        s.Shelf.QRCodePath
                    )
                )).ToList()))
            .FirstOrDefaultAsync();

        if (product == null)
        {
            throw new KeyNotFoundException("Product not found");
        }

        return product;
    }

    // Create Product
    public async Task<GetProductDto> CreateProductAsync(CreateProductDto createProductDto)
    {
        if (await _context.Products.AnyAsync(p => p.SKU == createProductDto.SKU))
        {
            throw new InvalidOperationException("Produk dengan SKU " + createProductDto.SKU + " sudah ada");
        }

        var product = new Models.Product
        {
            SKU = createProductDto.SKU,
            ProductName = createProductDto.ProductName,
            Barcode = createProductDto.Barcode,
            UnitOfMeasure = createProductDto.UnitOfMeasure,
            CreatedAt = DateTime.UtcNow
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return new GetProductDto(
            product.Id,
            product.SKU,
            product.ProductName,
            product.Barcode,
            product.UnitOfMeasure);
    }

    // Update Product
    public async Task<GetProductDto> UpdateProductAsync(int productId, UpdateProductDto updateProductDto)
    {
        var product = await _context.Products.FindAsync(productId);
        if (product == null)
        {
            throw new KeyNotFoundException("Product not found");
        }

        if (product.SKU != updateProductDto.SKU && await _context.Products.AnyAsync(p => p.SKU == updateProductDto.SKU))
        {
            throw new InvalidOperationException("Produk dengan SKU " + updateProductDto.SKU + " sudah ada");
        }

        product.SKU = updateProductDto.SKU ?? product.SKU;
        product.ProductName = updateProductDto.ProductName ?? product.ProductName;
        product.Barcode = updateProductDto.Barcode ?? product.Barcode;
        product.UnitOfMeasure = updateProductDto.UnitOfMeasure ?? product.UnitOfMeasure;
        product.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return new GetProductDto(
            product.Id,
            product.SKU,
            product.ProductName,
            product.Barcode,
            product.UnitOfMeasure);
    }

    // Delete Product
    public async Task DeleteProductAsync(int productId)
    {
        var product = await _context.Products.FindAsync(productId);
        if (product == null)
        {
            throw new KeyNotFoundException("Product not found");
        }

        product.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }
}