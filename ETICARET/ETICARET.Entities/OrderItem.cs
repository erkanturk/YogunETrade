namespace ETICARET.Entities
{
    public class OrderItem
    {
        public int Id { get; set; } // Sipariş öğesinin benzersiz kimliği
        public int OrderId { get; set; } // Bağlı olduğu sipariş kimliği
        public Order Order { get; set; } // Siparişle ilişkilendirme
        public Product Product { get; set; } // Ürünle ilişkilendirme
        public int ProductId { get; set; } // Ürünün kimliği
        public decimal Price { get; set; } // Ürünün fiyatı
        public int Quantity { get; set; } // Siparişteki ürün miktarı
    }
}
