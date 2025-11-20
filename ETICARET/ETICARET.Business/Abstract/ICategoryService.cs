using ETICARET.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETICARET.Business.Abstract
{
    public interface ICategoryService
    {
        Category GetById(int id); // Belirli bir kategoriye ait bilgileri getirir.
        Category GetByWithProducts(int id); // Kategoriye ait tüm ürünleri getirir.
        List<Category> GetAll(); // Tüm kategorileri getirir.
        void Create(Category entity); // Yeni kategori oluşturur.
        void Update(Category entity); // Kategori bilgilerini günceller.
        void Delete(Category entity); // Kategoriyi siler.
        void DeleteFromCategory(int categoryId, int productId); // Belirli bir ürünü kategoriden kaldırır.
    }
}
