using Catalog.Host.Data.Entities;

namespace Catalog.Host.Data.EntityConfigurations;

public class CatalogItemEntityTypeConfiguration
    : IEntityTypeConfiguration<CatalogItem>
{
    public void Configure(EntityTypeBuilder<CatalogItem> builder)
    {
        builder.ToTable("Catalog");

        builder.Property(ci => ci.Id)
            .UseHiLo("catalog_hilo")
            .IsRequired();

        builder.Property(ci => ci.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(ci => ci.Price)
            .IsRequired();

        builder.Property(ci => ci.PictureFileName)
            .IsRequired(false);

        builder.Property(ci => ci.Description)
            .IsRequired(false);

        builder.Property(ci => ci.PartNumber)
            .IsRequired(false);

        builder.HasOne(ci => ci.CatalogSubType)
            .WithMany()
            .HasForeignKey(ci => ci.CatalogSubTypeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ci => ci.CatalogModel)
            .WithMany()
            .HasForeignKey(ci => ci.CatalogModelId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}