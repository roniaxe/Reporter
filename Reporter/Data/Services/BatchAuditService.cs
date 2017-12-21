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

        public static Task<object> GetErrorGroups(BaseServiceModel serviceModel)
        {
            return BatchAuditRepository.GetErrorGroups(serviceModel);
        }

        public static Task<object> BatchStatistics(BaseServiceModel serviceModel)
        {
            return BatchAuditRepository.GetBatchStatistics(serviceModel);
        }

        public static Task<object> TaskList(BaseServiceModel serviceModel)
        {
            return BatchAuditRepository.GetTaskList(serviceModel);
        }

        public static Task<object> AllErrors(BaseServiceModel serviceModel)
        {
            return BatchAuditRepository.GetAllErrors(serviceModel);
        }
    }
}
