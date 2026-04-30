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
        // Load product with stocks and their shelves, then map to DTOs in memory
        var productEntity = await _context.Products
            .Include(p => p.Stocks)
                .ThenInclude(s => s.Shelves)
            .FirstOrDefaultAsync(p => p.Id == productId);

        if (productEntity == null)
        {
            throw new KeyNotFoundException("Product not found");
        }

        var stocks = productEntity.Stocks.Select(s => new GetStockDto(
            s.Id,
            s.ShelfId,
            s.ProductId,
            s.Quantity,
            s.Shelves != null
                ? new GetShelfForZoneDto(
                    s.Shelves.Id,
                    s.Shelves.ShelfCode,
                    s.Shelves.Aisle,
                    s.Shelves.Capacity,
                    s.Shelves.CurrentVolume,
                    s.Shelves.QRCodePath ?? string.Empty)
                : new GetShelfForZoneDto(0, string.Empty, 0, 0, 0, string.Empty)
        )).ToList();

        return new GetProductDetailDto(
            productEntity.Id,
            productEntity.SKU,
            productEntity.ProductName,
            productEntity.Barcode,
            productEntity.UnitOfMeasure,
            stocks);
    }

    // Create Product
    public async Task<GetProductDto> CreateProductAsync(CreateProductDto createProductDto)
    {
        if (await _context.Products.AnyAsync(p => p.SKU == createProductDto.SKU))
        {
            throw new InvalidOperationException("Produk dengan SKU " + createProductDto.SKU + " sudah ada");
        }

        var product = new Models.Products
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