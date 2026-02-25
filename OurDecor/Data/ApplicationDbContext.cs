using Microsoft.EntityFrameworkCore;
using OurDecor.Models;

namespace OurDecor.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) 
        {
        }

        public DbSet<ProductType> ProductTypes { get; set; }
        public DbSet<MaterialType> MaterialTypes { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Material> Materials { get; set; }
        public DbSet<ProductMaterial> ProductMaterials { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Уникальные индексы
            modelBuilder.Entity<ProductType>()
                .HasIndex(pt => pt.Name).IsUnique();

            modelBuilder.Entity<MaterialType>()
                .HasIndex(mt => mt.Name).IsUnique();

            modelBuilder.Entity<Product>()
                .HasIndex(p => p.Article).IsUnique();

            modelBuilder.Entity<Material>()
                .HasIndex(m => m.Name).IsUnique();

            modelBuilder.Entity<ProductMaterial>()
                .HasIndex(pm => new { pm.ProductId, pm.MaterialId }).IsUnique();

            // Индексы для ускорения поиска
            modelBuilder.Entity<Product>()
                .HasIndex(p => p.ProductTypeId);

            modelBuilder.Entity<Material>()
                .HasIndex(m => m.MaterialTypeId);

            modelBuilder.Entity<ProductMaterial>()
                .HasIndex(pm => pm.ProductId);

            modelBuilder.Entity<ProductMaterial>()
                .HasIndex(pm => pm.MaterialId);

            // Связи и каскадное удаление
            modelBuilder.Entity<Product>()
               .HasOne(p => p.ProductType)
               .WithMany(pt => pt.Products)
               .HasForeignKey(p => p.ProductTypeId)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Material>()
                .HasOne(m => m.MaterialType)
                .WithMany(mt => mt.Materials)
                .HasForeignKey(m => m.MaterialTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProductMaterial>()
                .HasOne(pm => pm.Product)
                .WithMany(p => p.ProductMaterials)
                .HasForeignKey(pm => pm.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProductMaterial>()
                .HasOne(pm => pm.Material)
                .WithMany(m => m.ProductMaterials)
                .HasForeignKey(pm => pm.MaterialId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
