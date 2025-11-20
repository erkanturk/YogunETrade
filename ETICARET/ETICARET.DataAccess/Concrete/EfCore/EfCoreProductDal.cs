using ETICARET.DataAccess.Abstract;
using ETICARET.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ETICARET.DataAccess.Concrete.EfCore
{
    public class EfCoreProductDal : EfCoreGenericRepository<Product, DataContext>, IProductDal
    {
        // Belirli bir kategoriye ait kaç ürün olduğunu hesaplar.
        public int GetCountByCategory(string category)
        {
            using (var context = new DataContext())
            {
                var products = context.Products.AsQueryable(); // Sorguyu LINQ ile işlenebilir hale getirir

                if (!string.IsNullOrEmpty(category) && category != "all")
                {
                    // Ürünleri, belirli bir kategoriye ait olup olmamasına göre filtreler.
                    products = products
                               .Include(i => i.ProductCategories) // Ürünün hangi kategorilere ait olduğunu getir
                               .ThenInclude(i => i.Category) // Kategorinin detaylarını da dahil et
                               .Where(i => i.ProductCategories.Any(a => a.Category.Name.ToLower() == category.ToLower()));

                    return products.Count(); // Filtrelenmiş ürünlerin sayısını döndür
                }
                else
                {
                    return products.Include(i => i.ProductCategories)
                                   .ThenInclude(i => i.Category)
                                   .Where(i => i.ProductCategories.Any())
                                   .Count();
                }
            }
        }

        // Bir ürünün tüm detaylarını getirir
        public Product GetProductDetails(int id)
        {
            using (var context = new DataContext())
            {
                return context.Products
                       .Where(i => i.Id == id) // Ürünü ID ile filtrele
                       .Include("Images") // Ürüne ait resimleri getir
                       .Include("Comments") // Ürüne ait yorumları getir
                       .Include(i => i.ProductCategories) // Ürünün kategori bilgilerini getir
                       .ThenInclude(i => i.Category) // Kategorinin detaylarını dahil et
                       .FirstOrDefault(); // İlk eşleşen ürünü döndür
            }
        }

        // Belirtilen kategoriye göre sayfalama ile ürünleri getirir.
        public List<Product> GetProductsByCategory(string category, int page, int pageSize)
        {
            using (var context = new DataContext())
            {
                var products = context.Products.Include("Images").AsQueryable(); 
                // Ürünleri ve resimlerini yükleyerek sorguya hazır hale getir

                if (!string.IsNullOrEmpty(category) && category != "all")
                {
                    products = products
                              .Include(i => i.ProductCategories)
                              .ThenInclude(i => i.Category)
                              .Where(i => i.ProductCategories.Any(a => a.Category.Name.ToLower() == category.ToLower()));
                }

                return products.Skip((page - 1) * pageSize).Take(pageSize).ToList(); // Sayfalama işlemi yaparak belirli sayıda ürün getir
            }
        }

        // Ürünü günceller ve yeni kategori bilgilerini set eder
        public void Update(Product entity, int[] categoryIds)
        {
            using (var context = new DataContext())
            {
                var products = context.Products.Include(i => i.ProductCategories).FirstOrDefault(i => i.Id == entity.Id);

                if (products is not null)
                {
                    products.Price = entity.Price;
                    products.Name = entity.Name;
                    products.Description = entity.Description;
                    // Ürün ile ilişkili kategorileri güncelle
                    products.ProductCategories = categoryIds.Select(catId => new ProductCategory()
                    {
                        ProductId = entity.Id,
                        CategoryId = catId,
                    }).ToList();
                    products.Images = entity.Images;
                }

                context.SaveChanges(); // Değişiklikleri kaydet
            }
        }

        // Ürünü ve ilişkili tüm resimlerini siler.
        public override void Delete(Product entity)
        {
            using (var context = new DataContext())
            {
                context.Images.RemoveRange(entity.Images); // Ürüne ait tüm resimleri sil
                context.Products.Remove(entity); // Ürünü sil
                context.SaveChanges();
            }
        }

        // Tüm ürünleri veya belirli bir filtreye uyanları getirir.
        public override List<Product> GetAll(Expression<Func<Product, bool>> filter = null)
        {
            using (var context = new DataContext())
            {
                return filter == null
                        ? context.Products.Include("Images").ToList() // Eğer filtre yoksa tüm ürünleri getir
                        : context.Products.Include("Images").Where(filter).ToList(); // Filtreye uyan ürünleri getir
            }
        }
    }
}
