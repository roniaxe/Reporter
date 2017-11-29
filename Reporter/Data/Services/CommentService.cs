using Reporter.Data.Repositories;
using Reporter.ViewModel;

namespace Reporter.Data.Services
{
    public static class CommentService
    {
        private static readonly ICommentRepository CommentRepository;

        static CommentService()
        {
            CommentRepository = new CommentRepository();
        }

        public static ErrorCommentConn Insert(ErrorCommentConn errorObj)
        {
            return CommentRepository.Insert(errorObj);
        }

        public static ErrorCommentConn GetById(object obj)
        {
            return CommentRepository.GetById(obj);
        }

        public static void Update(ErrorCommentConn errorObj)
        {
            CommentRepository.Update(errorObj);
        }
    }
}
