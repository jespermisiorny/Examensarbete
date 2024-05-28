using Examensarbete.Models;
using Microsoft.EntityFrameworkCore;

namespace Examensarbete.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : base(options)
        { }
        public DbSet<OrderData> OrderData { get; set; }
        public DbSet<Material> Materials { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Filter> Filters { get; set; }
        public DbSet<FilterCategory> FilterCategories { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<ProductMaterial> ProductMaterials { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //FilterCategories
            modelBuilder.Entity<FilterCategory>()
                .HasKey(fc => new { fc.FilterId, fc.CategoryId });

            modelBuilder.Entity<FilterCategory>()
                .HasOne(fc => fc.Filter)
                .WithMany(f => f.FilterCategories)
                .HasForeignKey(fc => fc.FilterId);

            modelBuilder.Entity<FilterCategory>()
                .HasOne(fc => fc.Category)
                .WithMany(c => c.FilterCategories)
                .HasForeignKey(fc => fc.CategoryId);


            //ProductCategory
            modelBuilder.Entity<ProductCategory>()
                .HasKey(pc => new { pc.ProductId, pc.CategoryId });

            modelBuilder.Entity<ProductCategory>()
                .HasOne(pc => pc.Product)
                .WithMany(p => p.ProductCategories)
                .HasForeignKey(pc => pc.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProductCategory>()
                .HasOne(pc => pc.Category)
                .WithMany(c => c.ProductCategories)
                .HasForeignKey(pc => pc.CategoryId);


            // ProductMaterial
            modelBuilder.Entity<ProductMaterial>()
                .HasKey(pm => new { pm.ProductId, pm.MaterialId });

            modelBuilder.Entity<ProductMaterial>()
                .HasOne(pm => pm.Product)
                .WithMany(p => p.ProductMaterials)
                .HasForeignKey(pm => pm.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProductMaterial>()
                .HasOne(pm => pm.Material)
                .WithMany(m => m.ProductMaterials)
                .HasForeignKey(pm => pm.MaterialId);


            // Orderdata
            modelBuilder.Entity<OrderData>(entity =>
            {
                entity.Property(e => e.OrderDate)
                .HasColumnType("date");
            });

            modelBuilder.Entity<OrderData>()
                    .HasOne(od => od.Product)
                    .WithMany(p => p.OrderDatas)
                    .HasForeignKey(od => od.ProductId)
                    .OnDelete(DeleteBehavior.Restrict); // setNull


            // Kategorier
            modelBuilder.Entity<Category>()
                    .HasMany(c => c.SubCategories)
                    .WithOne(c => c.ParentCategory)
                    .HasForeignKey(c => c.ParentId);


            // Decimalhantering
            modelBuilder.Entity<OrderData>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<OrderData>()
                .Property(p => p.ConfirmedNetAmount)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<Product>()
                .Property(p => p.PricePerUnit)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<Material>()
                .Property(p => p.EFMaterialNew)
                .HasColumnType("decimal(18, 5)");

            modelBuilder.Entity<Material>()
                .Property(p => p.EFProductionProcess)
                .HasColumnType("decimal(18, 5)");

            modelBuilder.Entity<Material>()
                .Property(p => p.EFEoLIncineration)
                .HasColumnType("decimal(18, 5)");

            modelBuilder.Entity<Material>()
                .Property(p => p.EFEoLRecycling)
                .HasColumnType("decimal(18, 5)");

            modelBuilder.Entity<Material>()
                .Property(p => p.RecycledContentInMaterial)
                .HasColumnType("decimal(18, 5)");

            modelBuilder.Entity<Material>()
                .Property(p => p.EFMaterialRecycled)
                .HasColumnType("decimal(18, 5)");

            modelBuilder.Entity<Material>()
                .Property(p => p.RecyclingRateAtEoL)
                .HasColumnType("decimal(18, 5)");


        }
    }
}
