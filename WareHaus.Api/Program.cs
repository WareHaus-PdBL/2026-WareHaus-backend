using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using WareHaus.Api.Data;
using WareHaus.Api.Middleware;
using WareHaus.Api.Services;

var builder = WebApplication.CreateBuilder(args);

Env.Load(".env");
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
builder.Services.AddScoped<ReceivingService>();
builder.Services.AddScoped<SmartLogisticsService>();

builder.Services.AddCors(options => options.AddPolicy("AllowAll", p => p.AllowAnyOrigin()));

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

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