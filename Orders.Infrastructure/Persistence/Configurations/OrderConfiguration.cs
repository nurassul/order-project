using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orders.Domain.Orders;

namespace Orders.Infrastructure.Persistence.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
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