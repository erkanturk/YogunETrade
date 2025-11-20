using ETICARET.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETICARET.Business.Abstract
{
    public interface IProductService
    {
        Product GetById(int id); // Belirli bir ürünü getirir.
        List<Product> GetProductByCategory(string category, int page, int pageSize); // Sayfalama ile belirli bir kategorideki ürünleri getirir.
        List<Product> GetAll(); // Tüm ürünleri getirir.
        Product GetProductDetail(int id); // Ürün detaylarını getirir.
        void Create(Product entity); // Yeni ürün ekler.
        void Update(Product entity, int[] categoryIds); // Ürünü ve kategorilerini günceller.
        void Delete(Product entity); // Ürünü siler.
        int GetCountByCategory(string category); // Belirli bir kategoride kaç ürün olduğunu hesaplar.
    }
}
