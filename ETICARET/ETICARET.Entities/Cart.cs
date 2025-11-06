using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETICARET.Entities
{
    public class Cart
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public List<CartItem> CartItems { get; set; }
    }
    public class CartItem
    {
        public int Id { get; set; }
        public int ProductId { get; set; }//Foreign key ilişkisi
        public Product Product { get; set; } //Navigator property
        public Cart Cart { get; set; }
        public int CartId { get; set; }
        public int Quantity { get; set; }

    }
}
