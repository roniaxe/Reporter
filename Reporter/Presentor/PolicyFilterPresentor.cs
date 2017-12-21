using Reporter.Data.Services;
using Reporter.View;
using Reporter.ViewModel.ServiceModel;

namespace Reporter.Presentor
{
    public class PolicyFilterPresentor
    {
        private readonly IPolicyFilterView _view;

        public PolicyFilterPresentor(IPolicyFilterView view)
        {
            _view = view;
            InitComponents();
        }

        private void InitComponents()
        {
            _view.RunButtonPressed += RunButtonActionAsync;
        }

        private async void RunButtonActionAsync()
        {
            _view.MainGrid.DataSource = null;

            var serviceModel = new AuditByPolicyServiceModel(
                _view.FromDate.Value.Date,
                _view.ToDate.Value.Date,
                _view.PolicyNo.Text,
                _view.ExtPolicyNo.Text,
                _view.ClientNo.Text);

            var results = await PolicyFilterService.AuditForPolicy(serviceModel);
            _view.MainGrid.DataSource = results;
        }
    }
}
