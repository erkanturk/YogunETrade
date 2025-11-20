using ETICARET.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETICARET.Business.Abstract
{
    public interface ICommentService
    {
        Comment GetById(int id); // Belirtilen yorumun bilgilerini getirir.
        void Create(Comment entity); // Yeni yorum ekler.
        void Update(Comment entity); // Yorumu günceller.
        void Delete(Comment entity); // Yorumu siler.
    }
}
