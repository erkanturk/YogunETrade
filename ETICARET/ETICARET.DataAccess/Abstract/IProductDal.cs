using ETICARET.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETICARET.DataAccess.Abstract
{
    public interface IProductDal : IRepository<Product>
    {
        int GetCountByCategory(string category); // Belirtilen kategoride kaç ürün olduğunu döndürür
        Product GetProductDetails(int id); // Bir ürünün detaylarını getirir
        List<Product> GetProductsByCategory(string category, int page, int pageSize); // Sayfalama ile belirli bir kategorideki ürünleri getirir
        void Update(Product entity, int[] categoryIds); // Ürünü ve ona bağlı kategorileri günceller
    }
}
