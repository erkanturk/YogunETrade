using ETICARET.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETICARET.Business.Abstract
{
    public interface ICartService
    {
        void InitialCart(string userId); // Kullanıcı için yeni bir sepet oluşturur.
        Cart GetCartByUserId(string userId); // Kullanıcının mevcut sepetini getirir.
        void AddToCart(string userId, int productId, int quantity); // Sepete ürün ekler.
        void DeleteFromCart(string userId, int productId); // Sepetten belirli bir ürünü kaldırır.
        void ClearCart(string cartId); // Kullanıcının sepetini tamamen temizler.
        /*
        Sepet işlemlerini Controller'dan bağımsız olarak yönetmek için bir servis katmanı gereklidir.
Kullanıcı giriş yaptığında otomatik sepet oluşturma, ürün ekleme/çıkarma işlemleri bu arayüzde tanımlanır.

        */
    }
}
