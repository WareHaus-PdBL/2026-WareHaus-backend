using DotNetEnv;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WareHaus.Api.Data;
using WareHaus.Api.Middleware;
using WareHaus.Api.Services;

var builder = WebApplication.CreateBuilder(args);

Env.Load("../.env");

var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION");

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("DB_CONNECTION belum diatur di file .env.");
}

builder.Services.AddOpenApi();

builder.Services.AddControllers();

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(entry => entry.Value?.Errors.Count > 0)
            .Where(entry => !IsRootDtoError(entry.Key))
            .ToDictionary(
                entry => FormatFieldName(entry.Key),
                entry => entry.Value!.Errors
                    .Select(error => FormatValidationMessage(entry.Key, error.ErrorMessage))
                    .ToArray()
            );

        var response = new
        {
            status = StatusCodes.Status400BadRequest,
            title = "Validation failed",
            message = "Request tidak valid. Periksa detail error.",
            errors
        };

        return new BadRequestObjectResult(response);
    };
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
// builder.Services.AddProblemDetails();

builder.Services.AddScoped<IZoneService, ZoneService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<DashboardService>();
builder.Services.AddScoped<PurchaseOrderService>();
builder.Services.AddScoped<ReceivingService>();
builder.Services.AddScoped<SmartLogisticsService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(connectionString);
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.UseStaticFiles();

app.UseCors("AllowAll");

// app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
}

app.MapControllers();

app.Run();

static bool IsRootDtoError(string fieldName)
{
    var rootDtoNames = new[]
    {
        "createZoneDto",
        "updateZoneDto",
        "createProductDto",
        "updateProductDto",
        "addStockLocationDto",
        "updateStockLocationDto",
        "addProductStockLocationDto",
        "updateProductStockLocationDto"
    };

    return rootDtoNames.Contains(fieldName);
}

static string FormatFieldName(string fieldName)
{
    return fieldName
        .Replace("$.", "")
        .Replace("createZoneDto.", "")
        .Replace("updateZoneDto.", "")
        .Replace("createProductDto.", "")
        .Replace("updateProductDto.", "")
        .Replace("addStockLocationDto.", "")
        .Replace("updateStockLocationDto.", "")
        .Replace("addProductStockLocationDto.", "")
        .Replace("updateProductStockLocationDto.", "");
}

static string FormatValidationMessage(string fieldName, string errorMessage)
{
    var cleanFieldName = FormatFieldName(fieldName);

    if (cleanFieldName.Equals("totalAisle", StringComparison.OrdinalIgnoreCase) &&
        errorMessage.Contains("could not be converted", StringComparison.OrdinalIgnoreCase))
    {
        return "Total aisle harus berupa bilangan bulat, contoh: 1 bukan 1.5.";
    }

    if (cleanFieldName.Equals("shelfPerAisle", StringComparison.OrdinalIgnoreCase) &&
        errorMessage.Contains("could not be converted", StringComparison.OrdinalIgnoreCase))
    {
        return "Shelf per aisle harus berupa bilangan bulat, contoh: 2 bukan 2.5.";
    }

    if (cleanFieldName.Equals("capacityPerShelf", StringComparison.OrdinalIgnoreCase) &&
        errorMessage.Contains("could not be converted", StringComparison.OrdinalIgnoreCase))
    {
        return "Capacity per shelf harus berupa bilangan bulat, contoh: 20 bukan 20.5.";
    }

    if (cleanFieldName.Equals("quantity", StringComparison.OrdinalIgnoreCase) &&
        errorMessage.Contains("could not be converted", StringComparison.OrdinalIgnoreCase))
    {
        return "Quantity harus berupa bilangan bulat.";
    }

    if (errorMessage.Contains("could not be converted", StringComparison.OrdinalIgnoreCase))
    {
        return $"{cleanFieldName} memiliki tipe data yang tidak sesuai.";
    }

    return errorMessage;
}