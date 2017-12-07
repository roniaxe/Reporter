using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Reporter.Data.Repositories;
using Reporter.Model;
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

        public static Task<object> GetErrorGroups(IReportView view, string connString)
        {
            return BatchAuditRepository.GetErrorGroups(view, connString);
        }

        public static Task<object> BatchStatistics(IReportView view, string connString)
        {
            return BatchAuditRepository.GetBatchStatistics(view, connString);
        }

        public static Task<object> TaskList(IReportView view, string connString)
        {
            return BatchAuditRepository.GetTaskList(view, connString);
        }

        public static Task<object> AllErrors(IReportView view, string connString)
        {
            return BatchAuditRepository.GetAllErrors(view, connString);
        }
    }
}
