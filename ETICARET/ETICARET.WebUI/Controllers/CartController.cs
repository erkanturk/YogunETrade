using ETICARET.Business.Abstract;
using ETICARET.Entities;
using ETICARET.WebUI.Extensions;
using ETICARET.WebUI.Identity;
using ETICARET.WebUI.Models;
using Iyzipay;
using Iyzipay.Model;
using Iyzipay.Request;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ETICARET.WebUI.Controllers
{
    public class CartController : Controller
    {
        // Kullanıcıya ait sepet işlemleri (ürün ekleme, silme, sepet boşaltma vs.) için servis
        private ICartService _cartService;

        // Ürünlerle ilgili işlemler (veritabanından ürün detaylarını almak, vs.) için servis
        private IProductService _productService;

        // Sipariş (order) işlemleri (sipariş kaydetme, listeleme, vs.) için servis
        private IOrderService _orderService;

        // Kullanıcı yönetimi için UserManager
        private UserManager<ApplicationUser> _userManager;

        /// <summary>
        /// Constructor üzerinden servislerin ve UserManager'ın dependency injection ile alınması.
        /// </summary>
        /// <param name="cartService">Sepet servisi</param>
        /// <param name="productService">Ürün servisi</param>
        /// <param name="orderService">Sipariş servisi</param>
        /// <param name="userManager">Kullanıcı yönetimi</param>
        public CartController(ICartService cartService, IProductService productService, IOrderService orderService, UserManager<ApplicationUser> userManager)
        {
            _cartService = cartService;
            _productService = productService;
            _orderService = orderService;
            _userManager = userManager;
        }

        /// <summary>
        /// Kullanıcının sepetini görüntüleyen sayfa (GET).
        /// </summary>
        /// <returns>Sepet içeriğini CartModel üzerinden görüntüleyen bir view döndürür.</returns>
        public IActionResult Index()
        {
            // Aktif kullanıcının sepeti çekilir
            var cart = _cartService.GetCartByUserId(_userManager.GetUserId(User));

            // Cart içerisindeki ürünleri CartModel'e map'leyerek View'a gönderiyoruz
            return View(
                new CartModel()
                {
                    CartId = cart.Id,
                    CartItems = cart.CartItems.Select(x => new CartItemModel()
                    {
                        CartItemId = x.Id,
                        ProductId = x.Product.Id,
                        Name = x.Product.Name,
                        Price = x.Product.Price,
                        ImageUrl = x.Product.Images[0].ImageUrl,
                        Quantity = x.Quantity
                    }).ToList()
                }
            );
        }

        /// <summary>
        /// Sepete ürün ekleme işlemini yapar.
        /// </summary>
        /// <param name="productId">Eklenecek ürünün ID değeri</param>
        /// <param name="quantity">Eklenecek ürün adedi</param>
        /// <returns>İşlem tamamlanınca kullanıcıyı sepet sayfasına yönlendirir.</returns>
        public IActionResult AddToCart(int productId, int quantity)
        {
            // Kullanıcının sepetine belirtilen ürünü, belirtilen adetle ekliyor
            _cartService.AddToCart(_userManager.GetUserId(User), productId, quantity);
            // Sepet sayfasına yönlendiriyor
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Sepetten ürün silme işlemini yapar.
        /// </summary>
        /// <param name="productId">Silinecek ürünün ID değeri</param>
        /// <returns>İşlem tamamlanınca kullanıcıyı sepet sayfasına yönlendirir.</returns>
        public IActionResult DeleteFromCart(int productId)
        {
            // Kullanıcının sepetinden belirtilen ürünü siler
            _cartService.DeleteFromCart(_userManager.GetUserId(User), productId);
            // Sepet sayfasına yönlendirir
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Satın alma (checkout) ekranını gösterir.
        /// </summary>
        /// <returns>Checkout için OrderModel içeren bir view döndürür.</returns>
        public IActionResult Checkout()
        {
            // Kullanıcının sepetini alıyoruz
            var cart = _cartService.GetCartByUserId(_userManager.GetUserId(User));

            // Sipariş modelini oluşturuyoruz
            OrderModel orderModel = new OrderModel();

            // Sipariş modelinin CartModel'ine sepet bilgilerini map'liyoruz
            orderModel.CartModel = new CartModel()
            {
                CartId = cart.Id,
                CartItems = cart.CartItems.Select(x => new CartItemModel()
                {
                    CartItemId = x.Id,
                    ProductId = x.Product.Id,
                    Name = x.Product.Name,
                    Price = x.Product.Price,
                    ImageUrl = x.Product.Images[0].ImageUrl,
                    Quantity = x.Quantity
                }).ToList()
            };

            // Checkout view'ına OrderModel gönderiyoruz
            return View(orderModel);
        }

        /// <summary>
        /// Satın alma işlemi (checkout) formundan gelen POST isteğidir.
        /// </summary>
        /// <param name="model">Sipariş oluşturmak için gerekli bilgiler</param>
        /// <param name="paymentMethod">Ödeme yöntemi (kredi kartı veya eft)</param>
        /// <returns>İşlem sonucu kullanıcıyı yine Checkout sayfasına yönlendirir ya da sepeti temizleyip sipariş başarı mesajı gösterir.</returns>
        [HttpPost]
        public IActionResult Checkout(OrderModel model, string paymentMethod)
        {
            // CartModel validasyonunu ModelState üzerinden kaldırıyoruz (çünkü burada manuel ekleniyor)
            ModelState.Remove("CartModel");

            // Model validasyonu (Örn: kullanıcı bilgileri, adres vs.)
            if (ModelState.IsValid)
            {
                // Kullanıcı ID'si alınır
                var userId = _userManager.GetUserId(User);

                // Kullanıcının sepeti alınır
                var cart = _cartService.GetCartByUserId(userId);

                // Sipariş modelinin CartModel'ine sepet bilgileri map'lenir
                model.CartModel = new CartModel()
                {
                    CartId = cart.Id,
                    CartItems = cart.CartItems.Select(x => new CartItemModel()
                    {
                        CartItemId = x.Id,
                        ProductId = x.Product.Id,
                        Name = x.Product.Name,
                        Price = x.Product.Price,
                        ImageUrl = x.Product.Images[0].ImageUrl,
                        Quantity = x.Quantity
                    }).ToList()
                };

                // Eğer ödeme yöntemi "credit" (kredi kartı) ise
                if (paymentMethod == "credit")
                {
                    // Iyzipay üzerinden ödeme işlemini gerçekleştiriyoruz
                    var payment = PaymentProccess(model);

                    // Ödeme başarılıysa
                    if (payment.Result.Status == "success")
                    {
                        // Siparişi kayıt altına al
                        SaveOrder(model, payment, userId);

                        // Sepeti temizle
                        ClearCart(cart.Id.ToString());

                        // Başarı mesajı
                        TempData.Put("message", new ResultModel()
                        {
                            Title = "Order Completed",
                            Message = "Your order has been completed successfully",
                            Css = "success"
                        });
                    }
                    else
                    {
                        // Ödeme başarısızsa kullanıcıya uyarı ver
                        TempData.Put("message", new ResultModel()
                        {
                            Title = "Order Uncompleted",
                            Message = "Your order could not be completed",
                            Css = "danger"
                        });
                    }
                }
                else
                {
                    // EFT veya farklı bir ödeme yöntemi ise
                    // Kredi kartıyla ödeme yok, direkt siparişi kaydediyoruz
                    SaveOrder(model, userId);
                    // Sepeti temizliyoruz
                    ClearCart(cart.Id.ToString());

                    // Başarı mesajı
                    TempData.Put("message", new ResultModel()
                    {
                        Title = "Order Completed",
                        Message = "Your order has been completed successfully",
                        Css = "success"
                    });
                }
            }

            // Model valid değilse veya ödeme başarısızsa tekrar aynı view'a dönüyoruz
            return View(model);
        }

        /// <summary>
        /// Sepeti (cart) temizler.
        /// </summary>
        /// <param name="id">Temizlenecek sepetin ID değeri</param>
        private void ClearCart(string id)
        {
            _cartService.ClearCart(id);
        }

        /// <summary>
        /// Ödeme yöntemi EFT ise siparişi veritabanına kaydeder.
        /// </summary>
        /// <param name="model">Sipariş bilgileri</param>
        /// <param name="userId">Kullanıcının ID'si</param>
        private void SaveOrder(OrderModel model, string userId)
        {
            // Yeni bir Order nesnesi oluşturup veritabanına kaydediyoruz
            Order order = new Order()
            {
                OrderNumber = Guid.NewGuid().ToString(),
                OrderState = EnumOrderState.completed,
                PaymentTypes = EnumPaymentTypes.Eft,
                PaymentToken = Guid.NewGuid().ToString(),
                ConversionId = Guid.NewGuid().ToString(),
                PaymentId = Guid.NewGuid().ToString(),
                OrderNote = model.OrderNote,
                OrderDate = DateTime.Now,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Address = model.Address,
                City = model.City,
                Email = model.Email,
                Phone = model.Phone,
                UserId = userId
            };

            // Sepetteki ürünleri siparişe OrderItem olarak ekliyoruz
            foreach (var cartItem in model.CartModel.CartItems)
            {
                var orderItem = new Entities.OrderItem()
                {
                    Price = cartItem.Price,
                    Quantity = cartItem.Quantity,
                    ProductId = cartItem.ProductId,
                };

                order.OrderItems.Add(orderItem);
            }

            // Order servis aracılığıyla siparişi kaydet
            _orderService.Create(order);
        }

        /// <summary>
        /// Ödeme yöntemi kredi kartı ise siparişi veritabanına kaydeder.
        /// </summary>
        /// <param name="model">Sipariş bilgileri</param>
        /// <param name="payment">Iyzipay üzerinden dönen ödeme bilgisi</param>
        /// <param name="userId">Kullanıcının ID'si</param>
        private void SaveOrder(OrderModel model, Task<Payment> payment, string userId)
        {
            // Iyzipay'den dönen ödeme bilgilerini de Order içerisine kaydediyoruz
            Order order = new Order()
            {
                OrderNumber = Guid.NewGuid().ToString(),
                OrderState = EnumOrderState.completed,
                PaymentTypes = EnumPaymentTypes.CreditCard,
                PaymentToken = Guid.NewGuid().ToString(),
                ConversionId = payment.Result.ConversationId,
                PaymentId = payment.Result.PaymentId,
                OrderNote = model.OrderNote,
                OrderDate = DateTime.Now,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Address = model.Address,
                City = model.City,
                Email = model.Email,
                Phone = model.Phone,
                UserId = userId
            };

            // Sepetteki ürünleri siparişin OrderItems listesine ekliyoruz
            foreach (var cartItem in model.CartModel.CartItems)
            {
                var orderItem = new Entities.OrderItem()
                {
                    Price = cartItem.Price,
                    Quantity = cartItem.Quantity,
                    ProductId = cartItem.ProductId,
                };

                order.OrderItems.Add(orderItem);
            }

            // Order servis aracılığıyla veritabanına kaydet
            _orderService.Create(order);
        }

        /// <summary>
        /// Iyzipay üzerinden ödeme işlemini gerçekleştiren metod.
        /// </summary>
        /// <param name="model">Sipariş bilgileri (kredi kartı, kullanıcı adresi, vb.)</param>
        /// <returns>Ödemenin sonucunu (Iyzipay Payment nesnesi) döndürür</returns>
        private async Task<Payment> PaymentProccess(OrderModel model)
        {
            // Iyzipay sandbox ortamı için gerekli kimlik bilgileri
            Options options = new Options()
            {
                BaseUrl = "https://sandbox-api.iyzipay.com",
                ApiKey = "sandbox-cNnJEaoyNt0sCREL4nOq8PajTLQwWeXz",
                SecretKey = "sandbox-cmJxJfaGlVarqNV3c5ZQcMTwVNh8qswx"
            };

            // Kullanıcının IP adresini almak için
            // IP adresi icanhazip.com'dan çekiliyor, gereksiz satır sonu karakterleri temizleniyor
            string externalIpString = new WebClient().DownloadString("http://icanhazip.com").Replace("\\r\\n", "").Replace("\\n", "").Trim();
            var externalIp = IPAddress.Parse(externalIpString);

            // Ödeme isteği oluşturuluyor
            CreatePaymentRequest request = new CreatePaymentRequest();
            request.Locale = Locale.TR.ToString(); // Türkçe işlem
            request.ConversationId = Guid.NewGuid().ToString(); // İşleme ait benzersiz ID
            request.Price = model.CartModel.TotalPrice().ToString().Split(',')[0]; // Toplam tutar (kuruş kısım ayırma)
            request.PaidPrice = model.CartModel.TotalPrice().ToString().Split(',')[0]; // Ödenen tutar
            request.Currency = Currency.TRY.ToString(); // Para birimi Türk Lirası
            request.Installment = 1; // Taksit sayısı
            request.BasketId = model.CartModel.CartId.ToString(); // Sepet Id
            request.PaymentGroup = PaymentGroup.PRODUCT.ToString(); // Ürün grubu
            request.PaymentChannel = PaymentChannel.WEB.ToString(); // Web kanalı üzerinden ödeme

            // Kredi kartı bilgileri
            PaymentCard paymentCard = new PaymentCard()
            {
                CardHolderName = model.CardName,
                CardNumber = model.CardNumber,
                ExpireYear = model.ExprationYear,
                ExpireMonth = model.ExprationMonth,
                Cvc = model.CVV,
                RegisterCard = 0 // Kayıtlı kart olarak eklenmesin
            };

            // Request'e kredi kartı bilgisi ekleniyor
            request.PaymentCard = paymentCard;

            // Ödeme yapan (Buyer) bilgileri
            Buyer buyer = new Buyer()
            {
                Id = _userManager.GetUserId(User),
                Name = model.FirstName,
                Surname = model.LastName,
                GsmNumber = model.Phone,
                Email = model.Email,
                IdentityNumber = "11111111111", // Örnek kimlik numarası
                RegistrationAddress = model.Address,
                Ip = externalIp.ToString(),
                City = model.City,
                Country = "TURKEY",
                ZipCode = "34000"
            };

            request.Buyer = buyer;

            // Fatura adresi ve gönderim adresi aynı kabul ediliyor
            Address address = new Address()
            {
                ContactName = model.FirstName + " " + model.LastName,
                City = model.City,
                Country = "Turkey",
                Description = model.Address,
                ZipCode = "34000"
            };

            request.BillingAddress = address;
            request.ShippingAddress = address;

            // Sepetteki ürünleri Iyzipay basket item listesine dönüştürür
            List<BasketItem> basketItems = new List<BasketItem>();
            BasketItem basketItem;

            foreach (var cartItem in model.CartModel.CartItems)
            {
                basketItem = new BasketItem()
                {
                    Id = cartItem.ProductId.ToString(),
                    Name = cartItem.Name,
                    Category1 = _productService.GetProductDetail(cartItem.ProductId).ProductCategories.FirstOrDefault().CategoryId.ToString(),
                    ItemType = BasketItemType.PHYSICAL.ToString(),
                    Price = (cartItem.Price * cartItem.Quantity).ToString().Split(",")[0] // Ürün fiyatı x adet
                };

                basketItems.Add(basketItem);
            }

            request.BasketItems = basketItems;

            // Iyzipay Payment yaratılır (async)
            Payment payment = await Payment.Create(request, options);

            // Payment nesnesi döndürülür (içinde Status, PaymentId, ConversationId, vs. bilgileri var)
            return payment;
        }

        /// <summary>
        /// Kullanıcının verdiği siparişleri listeler.
        /// </summary>
        /// <returns>Kullanıcının sipariş listesini döndüren bir view</returns>
        public IActionResult GetOrders()
        {
            // Aktif kullanıcı ID alınır
            var userId = _userManager.GetUserId(User);

            // Kullanıcının siparişleri çekilir
            var orders = _orderService.GetOrders(userId);

            // View'a gönderilecek OrderListModel listesi hazırlanır
            var orderListModel = new List<OrderListModel>();

            // Her bir sipariş için OrderListModel oluşturup orderListModel listesine ekleriz
            OrderListModel orderModel;

            foreach (var order in orders)
            {
                orderModel = new OrderListModel();
                orderModel.OrderId = order.Id;
                orderModel.Address = order.Address;
                orderModel.OrderNumber = order.OrderNumber;
                orderModel.OrderDate = order.OrderDate;
                orderModel.OrderState = order.OrderState;
                orderModel.PaymentTypes = order.PaymentTypes;
                orderModel.OrderNote = order.OrderNote;
                orderModel.City = order.City;
                orderModel.Email = order.Email;
                orderModel.FirstName = order.FirstName;
                orderModel.LastName = order.LastName;
                orderModel.Phone = order.Phone;

                // Siparişteki ürünleri OrderItems listesine map'liyoruz
                orderModel.OrderItems = order.OrderItems.Select(i => new OrderItemModel()
                {
                    OrderItemId = i.Id,
                    Name = i.Product.Name,
                    Price = i.Product.Price,
                    Quantity = i.Quantity,
                    ImageUrl = i.Product.Images[0].ImageUrl
                }).ToList();

                // Listeye ekleniyor
                orderListModel.Add(orderModel);
            }

            // orderListModel'i View'a gönderiyoruz
            return View(orderListModel);
        }

    }
}
