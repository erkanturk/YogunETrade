using ETICARET.Business.Abstract;
using ETICARET.DataAccess.Abstract;
using ETICARET.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETICARET.Business.Concrete
{
    public class CommentManager : ICommentService
    {
        private ICommentDal _commentDal; // Veri erişim bağımlılığı

        public CommentManager(ICommentDal commentDal)
        {
            _commentDal = commentDal;
        }

        // Yorum ekler
        public void Create(Comment entity)
        {
            _commentDal.Create(entity);
        }

        // Yorumu siler
        public void Delete(Comment entity)
        {
            _commentDal.Delete(entity);
        }

        // Belirtilen yorum ID'sine göre getirir
        public Comment GetById(int id)
        {
            return _commentDal.GetById(id);
        }

        // Yorumu günceller
        public void Update(Comment entity)
        {
            _commentDal.Update(entity);
        }
    }
}
