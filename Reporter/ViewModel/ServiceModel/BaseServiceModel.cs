using System;

namespace Reporter.ViewModel.ServiceModel
{
    public class BaseServiceModel
    {
        public BaseServiceModel(DateTime fromDate, DateTime toDate)
        {
            FromDate = fromDate;
            ToDate = toDate;
        }

        public BaseServiceModel()
        {
            
        }

        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
}
