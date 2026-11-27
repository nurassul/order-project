using Deliveries.Domain.Models;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Deliveries.Infrastructure.Persistance;

/// <summary>
/// Предоставляет доступ к базе данных доставок.
/// </summary>
public class DeliveryDbContext : DbContext
{
    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="DeliveryDbContext"/>.
    /// </summary>
    /// <param name="options">Настройки контекста базы данных.</param>
    public DeliveryDbContext(DbContextOptions<DeliveryDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Возвращает набор доставок для выполнения запросов и изменения данных.
    /// </summary>
    public DbSet<Delivery> Deliveries => Set<Delivery>();


    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(DeliveryDbContext).Assembly);

        modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();
        modelBuilder.AddOutboxStateEntity();
    }
}
