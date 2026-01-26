using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Mapping;

public sealed class SaleItemConfiguration : IEntityTypeConfiguration<SaleItem>
{
    public void Configure(EntityTypeBuilder<SaleItem> builder)
    {
        builder.ToTable("SaleItems");

        // PK
        builder.HasKey(i => i.Id);

        builder.Property(i => i.Id)
            .HasColumnType("uuid")
            .HasDefaultValueSql("gen_random_uuid()");

        // Quantity
        builder.Property(i => i.Quantity)
            .IsRequired();

        // UnitPrice
        builder.Property(i => i.UnitPrice)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(i => i.ProductName)
            .HasColumnType("varchar(150)")
            .HasMaxLength(150)
            .IsRequired();

        // Discount
        builder.Property(i => i.PercentageDiscount)
            .HasPrecision(5, 2)
            .IsRequired();

        // Ignore calculated property
        builder.Ignore(i => i.Total);

        // Product
        builder.Property(i => i.ProductId)
            .HasColumnType("uuid")
            .IsRequired();

        builder.HasOne(i => i.Product)
            .WithMany()
            .HasForeignKey(i => i.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        // FK shadow property
        builder.Property<Guid>("SaleId")
            .HasColumnType("uuid")
            .IsRequired();
    }
}