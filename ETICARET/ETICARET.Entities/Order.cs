using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETICARET.Entities
{
    public class Order
    {
        public int Id { get; set; } // Siparişin benzersiz kimliği
        public string OrderNumber { get; set; } // Sipariş numarası
        public DateTime OrderDate { get; set; } // Sipariş tarihi
        public string UserId { get; set; } // Siparişi veren kullanıcının kimliği
        public string FirstName { get; set; } // Kullanıcının adı
        public string LastName { get; set; } // Kullanıcının soyadı
        public string Address { get; set; } // Teslimat adresi
        public string City { get; set; } // Şehir bilgisi
        public string Phone { get; set; } // Kullanıcı telefonu
        public string Email { get; set; } // Kullanıcı emaili
        public string OrderNote { get; set; } // Kullanıcının sipariş notu
        public string PaymentId { get; set; } // Ödeme işlem kimliği
        public string PaymentToken { get; set; } // Ödeme doğrulama tokeni
        public string ConversionId { get; set; } // Ödeme dönüşüm kimliği
        public EnumOrderState OrderState { get; set; } // Siparişin mevcut durumu
        public EnumPaymentTypes PaymentTypes { get; set; } // Ödeme türü (kredi kartı, havale vb.)
        public List<OrderItem> OrderItems { get; set; } // Siparişe ait ürünler

        public Order()
        {
            OrderItems = new List<OrderItem>(); // Sipariş detaylarını içeren liste başlatılıyor
        }
    }

    public enum EnumOrderState
    {
        waiting = 0,  // Sipariş beklemede
        unpaid = 1,   // Ödeme alınmadı
        completed = 2 // Sipariş tamamlandı
    }

    public enum EnumPaymentTypes
    {
        CreditCard = 0, // Kredi Kartı ile ödeme
        Eft = 1         // Havale / EFT ile ödeme
    }
}
