using ETICARET.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETICARET.Business.Abstract
{
    public interface IOrderService
    {
        List<Order> GetOrders(string userId); // Kullanıcının sipariş geçmişini getirir.
        void Create(Order entity); // Yeni bir sipariş oluşturur.
    }
}
