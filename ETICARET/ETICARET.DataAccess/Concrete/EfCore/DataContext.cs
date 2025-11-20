using ETICARET.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETICARET.DataAccess.Concrete.EfCore
{
    public class DataContext : DbContext // Entity Framework Core'un DbContext sınıfından türetilmiştir
    {
        // Veritabanı bağlantı ayarlarını yapılandırır.
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=OGRETMEN\MSSQLSERVER01;Database=ETICARET;uid=sa;pwd=1;TrustServerCertificate=True");
        }

        // Veritabanındaki ilişkileri ve kuralları belirler.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ProductCategory tablosunda birleşik anahtar tanımlanıyor (ProductId ve CategoryId birlikte eşsiz olacak)
            modelBuilder.Entity<ProductCategory>().HasKey(c => new { c.ProductId, c.CategoryId });
        }

        // Veritabanındaki tabloların tanımları
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Order> Orders { get; set; }
    }
}
