using System.ComponentModel.DataAnnotations;

namespace ETICARET.Entities
{
    public class Product
    {
        public int Id { get; set; } // Ürünün benzersiz kimliği
        public string Name { get; set; } // Ürün adı
        public string Description { get; set; } // Ürün açıklaması
        public List<Image> Images { get; set; } // Ürüne ait resimler
        [Range(0, double.MaxValue, ErrorMessage = "Fiyat geçerli bir değer olmalıdır.")]
        public decimal Price { get; set; } // Ürün fiyatı
        public List<ProductCategory> ProductCategories { get; set; } // Kategorilerle ilişki
        public List<Comment> Comments { get; set; } // Ürün yorumları

        public Product()
        {
            Images = new List<Image>();
            ProductCategories = new List<ProductCategory>();
            Comments = new List<Comment>();
        }
    }
}
