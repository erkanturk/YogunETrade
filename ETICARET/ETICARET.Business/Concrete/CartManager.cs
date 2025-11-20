using ETICARET.Business.Abstract;
using ETICARET.DataAccess.Abstract;
using ETICARET.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETICARET.Business.Concrete
{
    public class CartManager : ICartService
    {
        private ICartDal _cartDal; // Veri erişim katmanı için bağımlılık

        public CartManager(ICartDal cartDal)
        {
            _cartDal = cartDal;
        }

        // Sepete ürün ekler
        public void AddToCart(string userId, int productId, int quantity)
        {
            var cart = GetCartByUserId(userId); // Kullanıcının mevcut sepetini getirir

            if (cart is not null)
            {
                var index = cart.CartItems.FindIndex(x => x.ProductId == productId);

                if (index < 0) // Ürün sepette yoksa yeni ekler
                {
                    cart.CartItems.Add(
                        new CartItem()
                        {
                            ProductId = productId,
                            Quantity = quantity,
                            CartId = cart.Id
                        }
                    );
                }
                else
                {
                    cart.CartItems[index].Quantity += quantity; // Ürün zaten sepette varsa miktarı artırır
                }
            }

            _cartDal.Update(cart); // Sepeti günceller
        }

        // Sepeti temizler
        public void ClearCart(string cartId)
        {
            _cartDal.ClearCart(cartId);
        }

        // Sepetten belirli bir ürünü siler
        public void DeleteFromCart(string userId, int productId)
        {
            var cart = GetCartByUserId(userId);
            if (cart != null)
            {
                _cartDal.DeleteFromCart(cart.Id, productId);
            }
        }

        // Kullanıcının sepetini getirir
        public Cart GetCartByUserId(string userId)
        {
            return _cartDal.GetCartByUserId(userId);
        }

        // Yeni bir kullanıcı için boş bir sepet oluşturur
        public void InitialCart(string userId)
        {
            Cart cart = new Cart()
            {
                UserId = userId,
            };
            _cartDal.Create(cart);
        }
    }
}
