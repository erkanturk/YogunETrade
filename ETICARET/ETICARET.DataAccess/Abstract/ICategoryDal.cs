using ETICARET.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETICARET.DataAccess.Abstract
{
    public interface ICategoryDal : IRepository<Category> // Genel Repository’den türetilmiştir
    {
        void DeleteFromCategory(int categoryId, int productId); // Bir ürünü belirtilen kategoriden kaldırır
        Category GetByIdWithProducts(int id); // Kategori ile birlikte o kategoriye ait ürünleri getirir
    }
}
/*
Kategori yönetimi için bazı özel işlemler gerektiğinden, örneğin bir kategoriye bağlı ürünleri getirmek için GetByIdWithProducts eklenmiştir.


*/