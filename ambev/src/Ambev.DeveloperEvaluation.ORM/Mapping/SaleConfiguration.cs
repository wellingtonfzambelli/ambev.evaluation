using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Mapping;

public sealed class SaleConfiguration : IEntityTypeConfiguration<Sale>
{
    public void Configure(EntityTypeBuilder<Sale> builder)
    {
        builder.ToTable("Sales");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id)
            .HasColumnType("uuid")
            .HasDefaultValueSql("gen_random_uuid()");


        builder.Property(s => s.SaleNumber)
            .IsRequired()
            .HasColumnType("varchar(50)")
            .HasMaxLength(50);

        builder.Property(s => s.SaleDate)
            .IsRequired();

        builder.Property(s => s.CreatedAt)
            .IsRequired();

        builder.Property(s => s.UpdateAt)
            .IsRequired(false);

        builder.Property(s => s.SaleStatus)
            .IsRequired()
            .HasConversion<string>()
            .HasColumnType("varchar(20)");

        builder.Ignore(s => s.TotalAmount);

        // ===== USER =====
        builder.Property(s => s.UserId)
            .HasColumnType("uuid")
            .IsRequired();

        builder.HasOne(s => s.User)
            .WithMany()
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // ===== BRANCH =====
        builder.Property(s => s.BranchId)
            .HasColumnType("uuid")
            .IsRequired();

        builder.HasOne(s => s.Branch)
            .WithMany()
            .HasForeignKey(s => s.BranchId)
            .OnDelete(DeleteBehavior.Restrict);

        // ===== ITEMS (Aggregate boundary) =====
        builder.HasMany(typeof(SaleItem), "_items")
            .WithOne()
            .HasForeignKey("SaleId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}