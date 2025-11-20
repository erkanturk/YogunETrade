using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETICARET.Entities
{
    public class Category
    {
        public int Id { get; set; } // Kategorinin benzersiz kimliği
        public string Name { get; set; } // Kategori adı
        public List<ProductCategory> ProductCategories { get; set; } // Bu kategoriye ait ürünlerle ilişkisi

        public Category()
        {
            ProductCategories = new List<ProductCategory>(); // İlişkilendirme listesi başlatılıyor
        }
    }
}
