using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reporter.Model;
using Reporter.ViewModel;

namespace Reporter.Data.Repositories
{
    public interface ICommentRepository : IGenericRepository<ErrorCommentConn>
    {
    }

    public class CommentRepository : ICommentRepository
    {
        public List<ErrorCommentConn> GetAll()
        {
            using (var db = new ReporterCompactModel())
            {
                return db.Comment.ToList();
            }
        }

        public ErrorCommentConn GetById(object obj)
        {
            using (var db = new ReporterCompactModel())
            {
                return db.Comment.FirstOrDefault(cmt => 
                cmt.ErrorMessage == ((ErrorCommentConn)obj).ErrorMessage &&
                cmt.Batch == ((ErrorCommentConn)obj).Batch &&
                cmt.Task == ((ErrorCommentConn)obj).Task);
            }
        }

        public ErrorCommentConn Insert(ErrorCommentConn obj)
        {
            using (var db = new ReporterCompactModel())
            {
                db.Comment.Add(obj);
                db.SaveChanges();
                return obj;
            }
        }

        public void Update(ErrorCommentConn obj)
        {
            using (var db = new ReporterCompactModel())
            {
                db.Comment.Attach(obj);
                db.Entry(obj).State = EntityState.Modified;
                db.SaveChanges();
            }
        }
    }
}
