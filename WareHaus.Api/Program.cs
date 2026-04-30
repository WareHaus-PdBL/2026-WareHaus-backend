using Microsoft.EntityFrameworkCore;
using WareHaus.Api.Data;
using WareHaus.Api;
using WareHaus.Api.Services;

var builder = WebApplication.CreateBuilder(args);

Env.Load();
var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION");

builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddScoped<IZoneService, ZoneService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<PurchaseOrderService>();

builder.Services.AddCors(options => options.AddPolicy("AllowAll", p => p.AllowAnyOrigin()));

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});

var app = builder.Build();

app.UseStaticFiles();
app.UseCors("AllowAll");

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
=======
}
