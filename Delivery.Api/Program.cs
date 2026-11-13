using Ardalis.Specification;
using Common.Application.Pagination;
using Delivery.Api.ExceptionsHandler;
using Delivery.Application.Deliveries.Repositories;
using Delivery.Application.Deliveries.Services;
using Delivery.Infrastructure.Deliveries.Messaging.Consumers;
using Delivery.Infrastructure.Deliveries.Repositories;
using Delivery.Infrastructure.Persistance;
using FluentValidation;
using MassTransit;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddScoped<IDeliveryService, DeliveryService>();
builder.Services.AddScoped<IDeliveryRepository, DeliveryRepository>();
builder.Services.AddScoped<IValidator<PaginatedRequest>, PaginatedRequestValidator>();

builder.Services.AddDbContext<DeliveryDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DeliveryDb")));


builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<OrderCreatedConsumer>();

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

        cfg.ReceiveEndpoint("delivery-orders", endpoint =>
        {
            endpoint.UseMessageRetry(retry =>
                retry.Interval(3, TimeSpan.FromSeconds(2)));

            endpoint.ConfigureConsumer<OrderCreatedConsumer>(context);
        });
    });
});

var app = builder.Build();

app.UseExceptionHandler();



if (app.Configuration.GetValue<bool>("Database:ApplyMigrations"))
{
    using var scope = app.Services.CreateScope();

    var db = scope.ServiceProvider
        .GetRequiredService<DeliveryDbContext>();

    await db.Database.MigrateAsync();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
