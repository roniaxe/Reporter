using System;
using System.Threading.Tasks;
using Reporter.Data.Repositories;
using Reporter.ViewModel.ServiceModel;

namespace Reporter.Data.Services
{
    public static class BatchAuditService
    {
        private static readonly IBatchAuditRepository BatchAuditRepository;

        static BatchAuditService()
        {
            BatchAuditRepository = new BatchAuditRepository();
        }

        public static Task<object> GetErrorGroups((DateTime FromDate, DateTime ToDate) serviceModel)
        {
            return BatchAuditRepository.GetErrorGroups(serviceModel);
        }

        public static Task<object> BatchStatistics((DateTime FromDate, DateTime ToDate) serviceModel)
        {
            return BatchAuditRepository.GetBatchStatistics(serviceModel);
        }

        public static Task<object> TaskList((DateTime FromDate, DateTime ToDate) serviceModel)
        {
            return BatchAuditRepository.GetTaskList(serviceModel);
        }

        public static Task<object> AllErrors((DateTime FromDate, DateTime ToDate) serviceModel)
        {
            return BatchAuditRepository.GetAllErrors(serviceModel);
        }
    }
}
