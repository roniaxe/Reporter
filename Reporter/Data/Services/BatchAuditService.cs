using Reporter.Data.Repositories;
using Reporter.View;

namespace Reporter.Data.Services
{
    public static class BatchAuditService
    {
        private static readonly IBatchAuditRepository BatchAuditRepository;

        static BatchAuditService()
        {
            BatchAuditRepository = new BatchAuditRepository();
        }

        public static object GetErrorGroups(IReportView view, string connString)
        {
            return BatchAuditRepository.GetErrorGroups(view, connString);
        }
    }
}
