using ETICARET.DataAccess.Abstract;
using ETICARET.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETICARET.DataAccess.Concrete.EfCore
{
    public class EfCoreOrderDal : EfCoreGenericRepository<Order, DataContext>, IOrderDal
    {
        public List<Order> GetOrders(string userId)
        {
           using (var context = new DataContext())
            {
                var orders = context.Orders
                    .Include(i => i.OrderItems)
                    .ThenInclude(i => i.Product)
                    .ThenInclude(i => i.Images)
                    .AsQueryable();//Filtreleme işlemleri için sorguyu linq üzerinde çalışabilir hale getir.
                if (!string.IsNullOrEmpty(userId))
                {
                    orders=orders.Where(i => i.UserId==userId);
                }
                return orders.ToList();
            }
        }
    }
}

/*
1️ Include(i => i.OrderItems)
Amaç: Sipariş içindeki ürünleri (OrderItems) de çekmek.
Neden? Eğer Include kullanılmazsa, yalnızca Orders tablosundaki temel sipariş bilgileri gelir. Siparişe ait ürünlerin listesi lazy loading nedeniyle null kalabilir veya ekstra sorgularla çağrılması gerekebilir. Bu yüzden doğrudan siparişe ait ürünleri dahil ediyoruz.
2️ ThenInclude(i => i.Product)
Amaç: Sipariş içindeki ürünlerin detaylarını da (Product) çekmek.
Neden? Eğer OrderItems içinde sadece ProductId gibi yabancı anahtarları çağırırsak, ürünün tüm detaylarını (isim, açıklama, fiyat vb.) göremeyiz.
Çözüm: ThenInclude kullanarak her sipariş ürününe ait Product bilgisini de getiriyoruz.
3️ ThenInclude(i => i.Images)
Amaç: Sipariş içindeki ürünlerin resimlerini de (Images) dahil etmek.
Neden? Eğer bir ürünün görselleri Product içinde saklanıyorsa, siparişteki ürünleri gösterirken ürüne ait resimleri de göstermek için bu bilgiyi dahil etmemiz gerekir.
4️ AsQueryable()
Amaç: LINQ işlemlerine devam edebilmek için sorgunun LINQ tarafından yönetilebilir hale getirilmesi.
Neden? AsQueryable() kullanılmazsa, veriler doğrudan veritabanından çekilip bir listeye dönüştürülür (ToList()). Ancak burada filtreleme (Where) işlemi yapılacağı için sorgunun veritabanına gönderilmeden önce LINQ tarafından yönetilebilmesini sağlıyoruz.
5️ Where(i => i.UserId == userId)
Amaç: Eğer userId parametresi boş değilse, yalnızca ilgili kullanıcıya ait siparişleri getir.
Neden? Eğer Where kullanmazsak, tüm siparişler getirilir. Ancak burada yalnızca belirli bir kullanıcıya ait siparişleri listelemek istediğimiz için filtreleme yapıyoruz.
*/