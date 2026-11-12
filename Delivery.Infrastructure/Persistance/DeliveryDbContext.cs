using Microsoft.EntityFrameworkCore;

namespace Delivery.Infrastructure.Persistance;

/// <summary>
///
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
    /// Возвращает получает набор доставок для выполнения запросов и изменения данных.
    /// </summary>
    public DbSet<Domain.Models.Delivery> Deliveries => Set<Domain.Models.Delivery>();


    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(DeliveryDbContext).Assembly);
    }
}
