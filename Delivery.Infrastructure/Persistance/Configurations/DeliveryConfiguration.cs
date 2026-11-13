using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Delivery.Infrastructure.Persistance.Configurations;

/// <summary>
/// Настраивает таблицу доставок, ключи и ограничения полей.
/// </summary>
public class DeliveryConfiguration : IEntityTypeConfiguration<Domain.Models.Delivery>
{
    /// <summary>
    /// Задаёт правила хранения сущности доставки.
    /// </summary>
    /// <param name="builder">Объект настройки сущности доставки.</param>
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
