using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETICARET.Entities
{
    public class Comment
    {
        public int Id { get; set; } // Yorumun benzersiz kimliği
        public string Text { get; set; } // Yorumun içeriği
        public int ProductId { get; set; } // Yorumun ait olduğu ürün kimliği
        public Product Product { get; set; } // İlgili ürün nesnesiyle ilişkilendirme
        public string UserId { get; set; } // Yorumu yapan kullanıcının kimliği
        public DateTime CreateOn { get; set; } // Yorumun oluşturulma tarihi
    }
}
