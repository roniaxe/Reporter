using System.Collections.Generic;
using System.Data;
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

        public static object GetErrorGroups(IReportView view, string connString)
        {
            return BatchAuditRepository.GetErrorGroups(view, connString);
        }

        public static DataTable BatchStatistics(IReportView view, string connString)
        {
            return BatchAuditRepository.BatchStatistics(view, connString);
        }
    }
}
