using MassTransit;
using Microsoft.EntityFrameworkCore;
using Orders.Domain.Orders;

namespace Orders.Infrastructure.Persistence;


/// <summary>
/// Предоставляет доступ к заказам и отслеживает изменения для сохранения в базе данных.
/// </summary>
public class OrderDbContext : DbContext
{

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="OrderDbContext"/>.
    /// </summary>
    /// <param name="options">Настройки контекста базы данных.</param>
    public OrderDbContext(DbContextOptions<OrderDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Возвращает получает набор заказов для выполнения запросов и изменения данных.
    /// </summary>
    public DbSet<Order> Orders => Set<Order>();


    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(OrderDbContext).Assembly);

        modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();
        modelBuilder.AddOutboxStateEntity();
    }
}
