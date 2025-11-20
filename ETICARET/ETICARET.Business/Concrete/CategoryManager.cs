using ETICARET.Business.Abstract;
using ETICARET.DataAccess.Abstract;
using ETICARET.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ETICARET.Business.Concrete
{
    public class CategoryManager : ICategoryService
    {
        private ICategoryDal _categoryDal; // Veri erişim katmanı bağımlılığı

        public CategoryManager(ICategoryDal categoryDal)
        {
            _categoryDal = categoryDal;
        }

        // Yeni kategori ekler
        public void Create(Category entity)
        {
            _categoryDal.Create(entity);
        }

        // Kategoriyi siler
        public void Delete(Category entity)
        {
            _categoryDal.Delete(entity);
        }

        // Belirtilen ürünü kategoriden kaldırır
        public void DeleteFromCategory(int categoryId, int productId)
        {
            _categoryDal.DeleteFromCategory(categoryId, productId);
        }

        // Tüm kategorileri getirir
        public List<Category> GetAll()
        {
            return _categoryDal.GetAll();
        }

        // ID'ye göre kategori getirir
        public Category GetById(int id)
        {
            return _categoryDal.GetById(id);
        }

        // Kategoriye bağlı ürünlerle birlikte getirir
        public Category GetByWithProducts(int id)
        {
            return _categoryDal.GetByIdWithProducts(id);
        }

        // Kategoriyi günceller
        public void Update(Category entity)
        {
            _categoryDal.Update(entity);
        }
    }
}
