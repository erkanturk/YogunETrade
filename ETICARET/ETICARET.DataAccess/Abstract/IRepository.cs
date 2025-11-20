using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ETICARET.DataAccess.Abstract
{
    public interface IRepository<T>
    {
        T GetById(int id);  // Belirtilen ID'ye sahip tek bir nesneyi getirir
        T GetOne(Expression<Func<T, bool>> filter = null); // Belirtilen koşula uyan tek bir nesneyi getirir
        List<T> GetAll(Expression<Func<T, bool>> filter = null); // Belirtilen koşula uyan tüm nesneleri getirir
        void Create(T entity); // Yeni bir nesne ekler
        void Update(T entity); // Var olan bir nesneyi günceller
        void Delete(T entity); // Belirtilen nesneyi siler
    }
}
/*
Bu yapı, tüm veri erişim katmanında ortak olan CRUD işlemlerinin 
tek bir yerden yönetilmesini sağlar. 
Generic olması sayesinde Product, Category, Order gibi farklı nesneler için tekrar tekrar yazılmak zorunda kalmaz.
*/