using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Orders.Application.Common.Models;
using Orders.Application.Common.Validation;
using Orders.Application.Orders.Models;
using Orders.Application.Orders.Repositories;
using Orders.Application.Orders.Services;
using Orders.Application.Orders.Validators;
using Orders.Infrastructure.Orders.Repositories;
using Orders.Infrastructure.Persistence;
using OrderService.ExceptionHandler;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();

builder.Services.AddOpenApi();
builder.Services.AddScoped<IOrderService, Orders.Application.Orders.Services.OrderService>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IValidator<OrderUpsertModel>, OrderUpsertModelValidator>();
builder.Services.AddScoped<IValidator<PaginatedRequest>, PaginatedRequestValidator>();

builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("OrdersDb")));

builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Configuration.GetValue<bool>("Database:ApplyMigrations"))
{
    using var scope = app.Services.CreateScope();

    var db = scope.ServiceProvider
        .GetRequiredService<OrderDbContext>();

    await db.Database.MigrateAsync();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}


app.UseAuthorization();

app.MapControllers();

app.Run();