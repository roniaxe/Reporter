using System;

namespace Reporter.ViewModel.ServiceModel
{
    public class AuditByPolicyServiceModel : BaseServiceModel
    {
        public string PolicyNo { get; set; }
        public string ExternalPolicyNo { get; set; }
        public string ClientNo { get; set; }

        public AuditByPolicyServiceModel(DateTime fromDate, DateTime toDate, string polNo, string exPolNo, string clientNo) 
            : base(fromDate, toDate)
        {
            PolicyNo = polNo;
            ExternalPolicyNo = exPolNo;
            ClientNo = clientNo;
        }

        public AuditByPolicyServiceModel()
        {
            
        }
    }
}
