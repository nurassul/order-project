using Common.Abstractions;
using Common.Application.Pagination;
using Common.Messaging;
using Deliveries.Api.ExceptionsHandler;
using Deliveries.Application.Deliveries.Repositories;
using Deliveries.Application.Deliveries.Services;
using Deliveries.Infrastructure.Deliveries.Messaging.Consumers;
using Deliveries.Infrastructure.Deliveries.Repositories;
using Deliveries.Infrastructure.Persistance;
using FluentValidation;
using MassTransit;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddScoped<IDeliveryService, DeliveryService>();
builder.Services.AddScoped<IDeliveryRepository, DeliveryRepository>();
builder.Services.AddScoped<IValidator<PaginatedRequest>, PaginatedRequestValidator>();
builder.Services.AddScoped<IEventPublisher, MassTransitEventPublisher>();

var deliveryConnectionString = builder.Configuration.GetConnectionString("DeliveryDb");
if (deliveryConnectionString == null)
{
    throw new InvalidOperationException("Не задана подключения DeliveryDb");
}

builder.Services.AddDbContext<DeliveryDbContext>(options =>
    options.UseNpgsql(deliveryConnectionString));


builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
builder.Services.AddExceptionHandler<InvalidDeliveryStatusExceptionHandler>();
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
                retry.Intervals(
                    TimeSpan.FromSeconds(1)
                    ));

            endpoint.ConfigureConsumer<OrderCreatedConsumer>(context);
        });
    });

    x.AddEntityFrameworkOutbox<DeliveryDbContext>(o =>
    {
        o.UsePostgres();

        o.UseBusOutbox();

        o.QueryDelay = TimeSpan.FromSeconds(1);
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
