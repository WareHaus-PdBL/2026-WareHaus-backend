using Microsoft.EntityFrameworkCore;
using WareHaus.Api.Data;
using WareHaus.Api.DTOs;
using WareHaus.Api.Models;

namespace WareHaus.Api.Services;

public class ProductService : IProductService
{
    private readonly AppDbContext _context;

    public ProductService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<GetProductDto>> GetAllProductsAsync()
    {
        return await _context.Products
            .AsNoTracking()
            .Where(product => product.DeletedAt == null)
            .Select(product => new GetProductDto(
                product.Id,
                product.SKU,
                product.ProductName,
                product.Barcode,
                product.UnitOfMeasure,
                product.Stocks
                    .Where(stock => stock.Quantity > 0)
                    .Sum(stock => (int?)stock.Quantity) ?? 0
            ))
            .ToListAsync();
    }

    public async Task<GetProductDetailDto> GetProductDetailsAsync(int productId)
    {
        return await BuildProductDetailDtoAsync(productId);
    }

    public async Task<GetProductDto> CreateProductAsync(CreateProductDto createProductDto)
    {
        if (await _context.Products.AnyAsync(product =>
                product.SKU == createProductDto.SKU &&
                product.DeletedAt == null))
        {
            throw new InvalidOperationException($"Produk dengan SKU {createProductDto.SKU} sudah ada.");
        }

        if (!string.IsNullOrWhiteSpace(createProductDto.Barcode) &&
            await _context.Products.AnyAsync(product =>
                product.Barcode == createProductDto.Barcode &&
                product.DeletedAt == null))
        {
            throw new InvalidOperationException($"Produk dengan barcode {createProductDto.Barcode} sudah ada.");
        }

        var product = new Products
        {
            SKU = createProductDto.SKU,
            ProductName = createProductDto.ProductName,
            Barcode = createProductDto.Barcode,
            UnitOfMeasure = createProductDto.UnitOfMeasure,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return new GetProductDto(
            product.Id,
            product.SKU,
            product.ProductName,
            product.Barcode,
            product.UnitOfMeasure,
            0
        );
    }

    public async Task<GetProductDto> UpdateProductAsync(int productId, UpdateProductDto updateProductDto)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(product =>
                product.Id == productId &&
                product.DeletedAt == null);

        if (product == null)
        {
            throw new KeyNotFoundException("Product not found.");
        }

        if (!string.IsNullOrWhiteSpace(updateProductDto.SKU) &&
            product.SKU != updateProductDto.SKU &&
            await _context.Products.AnyAsync(existingProduct =>
                existingProduct.Id != productId &&
                existingProduct.SKU == updateProductDto.SKU &&
                existingProduct.DeletedAt == null))
        {
            throw new InvalidOperationException($"Produk dengan SKU {updateProductDto.SKU} sudah ada.");
        }

        if (!string.IsNullOrWhiteSpace(updateProductDto.Barcode) &&
            product.Barcode != updateProductDto.Barcode &&
            await _context.Products.AnyAsync(existingProduct =>
                existingProduct.Id != productId &&
                existingProduct.Barcode == updateProductDto.Barcode &&
                existingProduct.DeletedAt == null))
        {
            throw new InvalidOperationException($"Produk dengan barcode {updateProductDto.Barcode} sudah ada.");
        }

        product.SKU = updateProductDto.SKU ?? product.SKU;
        product.ProductName = updateProductDto.ProductName ?? product.ProductName;
        product.Barcode = updateProductDto.Barcode ?? product.Barcode;
        product.UnitOfMeasure = updateProductDto.UnitOfMeasure ?? product.UnitOfMeasure;
        product.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        var currentStock = await _context.Stocks
            .AsNoTracking()
            .Where(stock =>
                stock.ProductId == productId &&
                stock.Quantity > 0)
            .SumAsync(stock => (int?)stock.Quantity) ?? 0;

        return new GetProductDto(
            product.Id,
            product.SKU,
            product.ProductName,
            product.Barcode,
            product.UnitOfMeasure,
            currentStock
        );
    }

    public async Task DeleteProductAsync(int productId)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(product =>
                product.Id == productId &&
                product.DeletedAt == null);

        if (product == null)
        {
            throw new KeyNotFoundException("Product not found.");
        }

        product.DeletedAt = DateTime.UtcNow;
        product.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }

    public async Task<List<GetProductStockLocationDto>> GetProductStockLocationsAsync(int productId)
    {
        var productDetail = await BuildProductDetailDtoAsync(productId);
        return productDetail.Stocks;
    }

    public async Task<GetProductDetailDto> AddProductStockLocationAsync(
        AddProductStockLocationDto addStockLocationDto
    )
    {
        await EnsureProductExistsAsync(addStockLocationDto.ProductId);

        var shelf = await GetShelfEntityAsync(addStockLocationDto.ShelfId);

        ValidateQuantity(addStockLocationDto.Quantity);

        var existingStock = await _context.Stocks
            .FirstOrDefaultAsync(stock =>
                stock.ProductId == addStockLocationDto.ProductId &&
                stock.ShelfId == addStockLocationDto.ShelfId);

        if (existingStock != null && existingStock.Quantity > 0)
        {
            throw new InvalidOperationException(
                "Produk sudah ada di shelf ini. Gunakan fitur update stock pada shelf lama."
            );
        }

        AddShelfVolume(shelf, addStockLocationDto.Quantity);

        if (existingStock == null)
        {
            var stock = new Stocks
            {
                ProductId = addStockLocationDto.ProductId,
                ShelfId = addStockLocationDto.ShelfId,
                Quantity = addStockLocationDto.Quantity,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Stocks.Add(stock);
        }
        else
        {
            existingStock.Quantity = addStockLocationDto.Quantity;
            existingStock.UpdatedAt = DateTime.UtcNow;
        }

        AddStockLog(
            addStockLocationDto.ProductId,
            addStockLocationDto.ShelfId,
            addStockLocationDto.Quantity,
            "StockIn",
            addStockLocationDto.Quantity
        );

        await _context.SaveChangesAsync();

        return await BuildProductDetailDtoAsync(addStockLocationDto.ProductId);
    }

    public async Task<GetProductDetailDto> UpdateProductStockLocationAsync(UpdateProductStockLocationDto dto)
    {
        if (dto.Quantity < 0)
        {
            throw new InvalidOperationException("Quantity tidak boleh kurang dari 0.");
        }

        await EnsureProductExistsAsync(dto.ProductId);

        var shelf = await GetShelfEntityAsync(dto.ShelfId);

        var stock = await _context.Stocks
            .FirstOrDefaultAsync(stock =>
                stock.ProductId == dto.ProductId &&
                stock.ShelfId == dto.ShelfId);

        if (stock == null)
        {
            throw new KeyNotFoundException("Stock product pada shelf ini tidak ditemukan.");
        }

        var oldQuantity = stock.Quantity;
        var newQuantity = dto.Quantity;
        var difference = newQuantity - oldQuantity;

        if (difference == 0)
        {
            return await BuildProductDetailDtoAsync(dto.ProductId);
        }

        if (difference > 0)
        {
            var availableCapacity = shelf.Capacity - shelf.CurrentVolume;

            if (difference > availableCapacity)
            {
                throw new InvalidOperationException("Kapasitas shelf tidak mencukupi.");
            }

            shelf.CurrentVolume += difference;

            AddStockLog(
                dto.ProductId,
                dto.ShelfId,
                difference,
                "StockIn",
                newQuantity
            );
        }
        else
        {
            var removedQuantity = Math.Abs(difference);

            if (removedQuantity > oldQuantity)
            {
                throw new InvalidOperationException("Quantity yang dikurangi melebihi stock yang tersedia.");
            }

            shelf.CurrentVolume -= removedQuantity;

            if (shelf.CurrentVolume < 0)
            {
                shelf.CurrentVolume = 0;
            }

            AddStockLog(
                dto.ProductId,
                dto.ShelfId,
                removedQuantity,
                "StockOut",
                newQuantity
            );
        }

        stock.Quantity = newQuantity;

        if (stock.Quantity == 0)
        {
            _context.Stocks.Remove(stock);
        }

        await _context.SaveChangesAsync();

        return await BuildProductDetailDtoAsync(dto.ProductId);
    }

    public async Task DeleteProductStockLocationAsync(int productId, int shelfId)
    {
        await EnsureProductExistsAsync(productId);

        var shelf = await GetShelfEntityAsync(shelfId);

        var stock = await _context.Stocks
            .FirstOrDefaultAsync(stock =>
                stock.ProductId == productId &&
                stock.ShelfId == shelfId);

        if (stock == null)
        {
            throw new KeyNotFoundException("Stock product pada shelf ini tidak ditemukan.");
        }

        var removedQuantity = stock.Quantity;

        ReduceShelfVolume(shelf, removedQuantity);

        AddStockLog(
            productId,
            shelfId,
            removedQuantity,
            "StockOut",
            0
        );

        _context.Stocks.Remove(stock);

        await _context.SaveChangesAsync();
    }

    private async Task<GetProductDetailDto> BuildProductDetailDtoAsync(int productId)
    {
        var product = await _context.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(product =>
                product.Id == productId &&
                product.DeletedAt == null);

        if (product == null)
        {
            throw new KeyNotFoundException("Product not found.");
        }

        var stocks = await _context.Stocks
            .AsNoTracking()
            .Include(stock => stock.Shelves!)
                .ThenInclude(shelf => shelf!.Zones)
            .Where(stock =>
                stock.ProductId == productId &&
                stock.Quantity > 0)
            .ToListAsync();

        var currentStock = stocks.Sum(stock => stock.Quantity);

        var productStocks = new List<GetProductStockLocationDto>();

        foreach (var stock in stocks)
        {
            if (stock.Shelves is not Shelves shelf || shelf.DeletedAt != null)
            {
                continue;
            }

            var zone = shelf.Zones;

            var capacity = GetShelfCapacity(shelf);
            var availableCapacity = Math.Max(0, capacity - shelf.CurrentVolume);

            var zoneId = zone?.Id ?? 0;
            var zoneCode = zone?.ZoneCode ?? string.Empty;
            var zoneName = zone?.ZoneName ?? string.Empty;

            var locationName = $"{zoneName} - Aisle {shelf.Aisle:00} - {shelf.ShelfCode}";

            productStocks.Add(new GetProductStockLocationDto(
                shelf.Id,
                shelf.ShelfCode,
                zoneId,
                zoneCode,
                zoneName,
                shelf.Aisle,
                locationName,
                stock.Quantity,
                capacity,
                shelf.CurrentVolume,
                availableCapacity,
                shelf.QRCodePath ?? string.Empty
            ));
        }

        return new GetProductDetailDto(
            product.Id,
            product.SKU,
            product.ProductName,
            product.Barcode,
            product.UnitOfMeasure,
            currentStock,
            productStocks
        );
    }

    private async Task EnsureProductExistsAsync(int productId)
    {
        var productExists = await _context.Products
            .AnyAsync(product =>
                product.Id == productId &&
                product.DeletedAt == null);

        if (!productExists)
        {
            throw new KeyNotFoundException("Product not found.");
        }
    }

    private async Task<Shelves> GetShelfEntityAsync(int shelfId)
    {
        var shelf = await _context.Shelves
            .Include(shelf => shelf.Stocks)
            .Include(shelf => shelf.Zones)
            .FirstOrDefaultAsync(shelf =>
                shelf.Id == shelfId &&
                shelf.DeletedAt == null);

        if (shelf == null)
        {
            throw new KeyNotFoundException("Shelf not found.");
        }

        return shelf;
    }

    private void AddStockLog(
        int productId,
        int shelfId,
        int quantity,
        string movementType,
        int stockAfterMovement
    )
    {
        var log = new StockLogs
        {
            ProductId = productId,
            ShelfId = shelfId,
            Quantity = quantity,
            MovementType = movementType,
            StockAfterMovement = stockAfterMovement,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.StockLogs.Add(log);
    }

    private static void ValidateQuantity(int quantity)
    {
        if (quantity <= 0)
        {
            throw new InvalidOperationException("Quantity harus lebih dari 0.");
        }
    }

    private static int GetShelfCapacity(Shelves shelf)
    {
        if (shelf.Capacity > 0)
        {
            return shelf.Capacity;
        }

        if (shelf.MaxCapacity > 0)
        {
            return shelf.MaxCapacity;
        }

        return 0;
    }

    private static void AddShelfVolume(Shelves shelf, int quantity)
    {
        var capacity = GetShelfCapacity(shelf);

        if (capacity <= 0)
        {
            throw new InvalidOperationException("Kapasitas shelf belum diatur.");
        }

        var availableCapacity = capacity - shelf.CurrentVolume;

        if (quantity > availableCapacity)
        {
            throw new InvalidOperationException(
                $"Kapasitas shelf tidak cukup. Sisa kapasitas saat ini: {availableCapacity}."
            );
        }

        shelf.CurrentVolume += quantity;
        shelf.CurrentCapacity = shelf.CurrentVolume;
        shelf.UpdatedAt = DateTime.UtcNow;

        if (shelf.MaxCapacity <= 0)
        {
            shelf.MaxCapacity = capacity;
        }
    }

    private static void ReduceShelfVolume(Shelves shelf, int quantity)
    {
        shelf.CurrentVolume = Math.Max(0, shelf.CurrentVolume - quantity);
        shelf.CurrentCapacity = shelf.CurrentVolume;
        shelf.UpdatedAt = DateTime.UtcNow;
    }

    public async Task<GetProductDetailDto> MoveProductStockLocationAsync(MoveProductStockLocationDto dto)
    {
        if (dto.Quantity <= 0)
        {
            throw new InvalidOperationException("Quantity harus lebih dari 0.");
        }

        await EnsureProductExistsAsync(dto.ProductId);

        if (dto.FromShelfId == dto.ToShelfId)
        {
            throw new InvalidOperationException("Source shelf dan destination shelf tidak boleh sama.");
        }

        var sourceShelf = await GetShelfEntityAsync(dto.FromShelfId);
        var destinationShelf = await GetShelfEntityAsync(dto.ToShelfId);

        var sourceStock = await _context.Stocks
            .FirstOrDefaultAsync(stock =>
                stock.ProductId == dto.ProductId &&
                stock.ShelfId == dto.FromShelfId);

        if (sourceStock == null || sourceStock.Quantity <= 0)
        {
            throw new KeyNotFoundException("Stock product pada source shelf tidak ditemukan.");
        }

        if (dto.Quantity > sourceStock.Quantity)
        {
            throw new InvalidOperationException("Quantity yang dipindahkan melebihi stock yang tersedia di source shelf.");
        }

        var availableCapacity = GetShelfCapacity(destinationShelf) - destinationShelf.CurrentVolume;

        if (dto.Quantity > availableCapacity)
        {
            throw new InvalidOperationException("Kapasitas destination shelf tidak mencukupi.");
        }

        sourceShelf.CurrentVolume = Math.Max(0, sourceShelf.CurrentVolume - dto.Quantity);
        sourceShelf.CurrentCapacity = sourceShelf.CurrentVolume;
        sourceShelf.UpdatedAt = DateTime.UtcNow;

        destinationShelf.CurrentVolume += dto.Quantity;
        destinationShelf.CurrentCapacity = destinationShelf.CurrentVolume;
        destinationShelf.UpdatedAt = DateTime.UtcNow;

        sourceStock.Quantity -= dto.Quantity;

        if (sourceStock.Quantity == 0)
        {
            _context.Stocks.Remove(sourceStock);
        }
        else
        {
            sourceStock.UpdatedAt = DateTime.UtcNow;
        }

        var destinationStock = await _context.Stocks
            .FirstOrDefaultAsync(stock =>
                stock.ProductId == dto.ProductId &&
                stock.ShelfId == dto.ToShelfId);

        if (destinationStock == null)
        {
            destinationStock = new Stocks
            {
                ProductId = dto.ProductId,
                ShelfId = dto.ToShelfId,
                Quantity = dto.Quantity,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Stocks.Add(destinationStock);
        }
        else
        {
            destinationStock.Quantity += dto.Quantity;
            destinationStock.UpdatedAt = DateTime.UtcNow;
        }

        AddStockLog(
            dto.ProductId,
            dto.FromShelfId,
            dto.Quantity,
            "MoveOut",
            sourceStock.Quantity
        );

        AddStockLog(
            dto.ProductId,
            dto.ToShelfId,
            dto.Quantity,
            "MoveIn",
            destinationStock.Quantity
        );

        await _context.SaveChangesAsync();

        return await BuildProductDetailDtoAsync(dto.ProductId);
    }
}


