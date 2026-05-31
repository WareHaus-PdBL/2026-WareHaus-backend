using DotNetEnv;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WareHaus.Api.Data;
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

// builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
// builder.Services.AddProblemDetails();

builder.Services.AddScoped<InboundServices>();
builder.Services.AddScoped<OutboundService>();

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
        "createPurchaseOrderDto",
        "receiveItemDto",
        "putawayDto",
        "createSalesOrderDto",
        "updateSalesOrderDto",
        "createPackingTaskDto",
        "verifyPackingItemDto",
        "completePackingTaskDto",
        "createShipmentDto"
    };

    return rootDtoNames.Contains(fieldName);
}

static string FormatFieldName(string fieldName)
{
    return fieldName
        .Replace("$.", "")
        .Replace("createPurchaseOrderDto.", "")
        .Replace("receiveItemDto.", "")
        .Replace("putawayDto.", "")
        .Replace("createSalesOrderDto.", "")
        .Replace("updateSalesOrderDto.", "")
        .Replace("createPackingTaskDto.", "")
        .Replace("verifyPackingItemDto.", "")
        .Replace("completePackingTaskDto.", "")
        .Replace("createShipmentDto.", "");
}

static string FormatValidationMessage(string fieldName, string errorMessage)
{
    var cleanFieldName = FormatFieldName(fieldName);

    if (cleanFieldName.Equals("productId", StringComparison.OrdinalIgnoreCase) &&
        errorMessage.Contains("could not be converted", StringComparison.OrdinalIgnoreCase))
    {
        return "Product id harus berupa bilangan bulat.";
    }

    if (cleanFieldName.Equals("poItemId", StringComparison.OrdinalIgnoreCase) &&
        errorMessage.Contains("could not be converted", StringComparison.OrdinalIgnoreCase))
    {
        return "PO item id harus berupa bilangan bulat.";
    }

    if (cleanFieldName.Equals("packingTaskId", StringComparison.OrdinalIgnoreCase) &&
        errorMessage.Contains("could not be converted", StringComparison.OrdinalIgnoreCase))
    {
        return "Packing task id harus berupa bilangan bulat.";
    }

    if (cleanFieldName.Equals("qtyExpected", StringComparison.OrdinalIgnoreCase) &&
        errorMessage.Contains("could not be converted", StringComparison.OrdinalIgnoreCase))
    {
        return "Qty expected harus berupa bilangan bulat.";
    }

    if (cleanFieldName.Equals("qtyReceived", StringComparison.OrdinalIgnoreCase) &&
        errorMessage.Contains("could not be converted", StringComparison.OrdinalIgnoreCase))
    {
        return "Qty received harus berupa bilangan bulat.";
    }

    if (cleanFieldName.Equals("qtyOrdered", StringComparison.OrdinalIgnoreCase) &&
        errorMessage.Contains("could not be converted", StringComparison.OrdinalIgnoreCase))
    {
        return "Qty ordered harus berupa bilangan bulat.";
    }

    if (cleanFieldName.Equals("qtyVerified", StringComparison.OrdinalIgnoreCase) &&
        errorMessage.Contains("could not be converted", StringComparison.OrdinalIgnoreCase))
    {
        return "Qty verified harus berupa bilangan bulat.";
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