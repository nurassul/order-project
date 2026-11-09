using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orders.Domain.Orders;

namespace Orders.Infrastructure.Persistence.Configurations;


/// <summary>
/// Настраивает отображение заказа на таблицу и ограничения её столбцов.
/// </summary>
public class OrderConfiguration : IEntityTypeConfiguration<Order>
{

    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");

        builder.HasKey(order => order.OrderId);

        builder.Property(order => order.CustomerName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(order => order.CustomerEmail)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(order => order.TotalAmount)
            .HasPrecision(18, 2);
    }
}
