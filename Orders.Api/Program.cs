using FluentValidation;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Orders.Api.ExceptionsHandler;
using Orders.Application.Common.Models;
using Orders.Application.Common.Validation;
using Orders.Application.Orders.Messaging;
using Orders.Application.Orders.Models;
using Orders.Application.Orders.Repositories;
using Orders.Application.Orders.Services;
using Orders.Application.Orders.Validators;
using Orders.Infrastructure.Orders.Messaging;
using Orders.Infrastructure.Orders.Repositories;
using Orders.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();

builder.Services.AddOpenApi();
builder.Services.AddScoped<IOrderService, Orders.Application.Orders.Services.OrderService>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IValidator<OrderUpsertModel>, OrderUpsertModelValidator>();
builder.Services.AddScoped<IValidator<PaginatedRequest>, PaginatedRequestValidator>();
builder.Services.AddScoped<IOrderEventPublisher, MassTransitOrderEventPublisher>();

builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("OrdersDb")));

builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(
            builder.Configuration["RabbitMq:Host"]!,
            "/",
            host =>
            {
                host.Username(
                    builder.Configuration["RabbitMq:Username"]!);

                host.Password(
                    builder.Configuration["RabbitMq:Password"]!);
            });
    });
});

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
