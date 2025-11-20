using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETICARET.Entities
{
    [Table("Image")] // Veritabanında 'Image' tablosuna karşılık gelir
    public class Image
    {
        public int Id { get; set; } // Resmin benzersiz kimliği
        public string? ImageUrl { get; set; } // Resmin URL adresi
        public int ProductId { get; set; } // Resmin ait olduğu ürün kimliği
        public Product Product { get; set; } // Ürünle ilişkilendirme
    }
}
