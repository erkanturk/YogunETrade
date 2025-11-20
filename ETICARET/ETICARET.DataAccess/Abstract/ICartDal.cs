using ETICARET.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETICARET.DataAccess.Abstract
{
    public interface ICartDal : IRepository<Cart> // Genel Repository’den türetilmiştir
    {
        void ClearCart(string cartId); // Belirtilen sepetteki tüm ürünleri temizler
        void DeleteFromCart(int cartId, int productId); // Belirtilen ürünü sepetteki belirli bir sepetten siler
        Cart GetCartByUserId(string userId); // Kullanıcının sepet bilgilerini getirir
    }
}
/*
👉 Neden var?
Sepet yönetimi için 
özel işlemler gerektiği için bu interface yazılmıştır. 
ClearCart tüm sepeti temizlerken, DeleteFromCart yalnızca belirli bir ürünü kaldırır.
*/