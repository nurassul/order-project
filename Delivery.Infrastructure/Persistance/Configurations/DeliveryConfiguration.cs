using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Delivery.Infrastructure.Persistance.Configurations;

/// <summary>
///
/// </summary>
public class DeliveryConfiguration : IEntityTypeConfiguration<Domain.Models.Delivery>
{
    /// <summary>
    ///
    /// </summary>
    /// <param name="builder"></param>
    public void Configure(EntityTypeBuilder<Domain.Models.Delivery> builder)
    {
        builder.ToTable("Deliveries");

        builder.HasKey(delivery => delivery.DeliveryId);

        builder.HasIndex(delivery => delivery.OrderId)
            .IsUnique();

        builder.Property(delivery => delivery.CustomerName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(delivery => delivery.CustomerEmail)
            .HasMaxLength(100)
            .IsRequired();
    }
}
