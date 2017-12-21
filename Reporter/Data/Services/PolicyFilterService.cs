using System;
using System.Threading.Tasks;
using Reporter.Data.Repositories;
using Reporter.View;
using Reporter.ViewModel.ServiceModel;

namespace Reporter.Data.Services
{
    public static class PolicyFilterService
    {
        private static readonly IBatchAuditRepository BatchAuditRepository;
        static PolicyFilterService()
        {
            BatchAuditRepository = new BatchAuditRepository();
        }

        public static Task<object> AuditForPolicy(AuditByPolicyServiceModel serviceModel)
        {
            return BatchAuditRepository.GetAuditByPolicy(serviceModel);
        }
    }
}
