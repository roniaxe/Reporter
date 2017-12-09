using System.Threading.Tasks;
using Reporter.Data.Repositories;
using Reporter.View;

namespace Reporter.Data.Services
{
    public static class PolicyFilterService
    {
        private static readonly IBatchAuditRepository BatchAuditRepository;
        static PolicyFilterService()
        {
            BatchAuditRepository = new BatchAuditRepository();
        }

        public static Task<object> AuditForPolicy(IPolicyFilterView view, string connString, string policyNo)
        {
            return BatchAuditRepository.GetAuditByPolicy(view, connString, policyNo);
        }
    }
}
