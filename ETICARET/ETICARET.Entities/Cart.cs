using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETICARET.Entities
{
    public class Cart
    {
        public int Id { get; set; } // Sepetin benzersiz kimliği
        public string UserId { get; set; } // Sepetin sahibinin kullanıcı kimliği
        public List<CartItem> CartItems { get; set; } // Sepette bulunan ürünler listesi
    }

    public class CartItem
    {
        public int Id { get; set; } // Sepet öğesinin benzersiz kimliği
        public int ProductId { get; set; } // Ürünün kimliği
        public Product Product { get; set; } // Ürün nesnesiyle ilişkilendirme
        public Cart Cart { get; set; } // Sepet nesnesiyle ilişkilendirme
        public int CartId { get; set; } // Sepet kimliği (ilişkili olduğu sepet)
        public int Quantity { get; set; } // Üründen kaç adet eklendiği bilgisi
    }
}
