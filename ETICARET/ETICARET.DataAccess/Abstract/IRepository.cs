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
        T GetById(int id);//Belirtilen ıd ye sahip tek bir nesneyi getirir
        T GetOne(Expression<Func<T, bool>> filter = null);//Belirtilen koşula uyan yek bir nesneyi getirir
        List<T> GetAll(Expression<Func<T, bool>> filter = null);//Belirtile koşula uyan tüm nesneleri getirir
        void Create(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
    /* 
     * Bu yapı tüm veri erişim katmanlarında olan ortak Crud işlemlerini tek bir yerden yönetilmesini sağlayacak
     * Generic olması sayesinde Product,Category,Order gibi farklı nesneler için tekrar tekrar yazılmak zorunda kalmayacak.
     
     
     */
}
