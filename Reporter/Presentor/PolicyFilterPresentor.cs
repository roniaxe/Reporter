using System.Threading.Tasks;
using Reporter.Data.Services;
using Reporter.View;

namespace Reporter.Presentor
{
    public class PolicyFilterPresentor
    {
        private readonly IPolicyFilterView _view;
        private readonly string _connString;

        public PolicyFilterPresentor(IPolicyFilterView view, string connString)
        {
            _view = view;
            _connString = connString;
            InitComponents();
        }

        private void InitComponents()
        {
            _view.RunButtonPressed += RunButtonActionAsync;
        }

        private async void RunButtonActionAsync()
        {
            _view.MainGrid.DataSource = null;
            var results = await PolicyFilterService.AuditForPolicy(_view, _connString, _view.PolicyNo.Text);
            _view.MainGrid.DataSource = results;
        }
    }
}
