using System;
using System.Windows.Forms;
using Reporter.Data.Services;
using Reporter.Forms;
using Reporter.Model;
using Reporter.View;

namespace Reporter.Presentor
{
    class AddEditPersonPresentor
    {
        private readonly IAddEditPersonView _view;
        private readonly ReportPresentor _sender;

        public AddEditPersonPresentor(IAddEditPersonView view, ReportPresentor sender)
        {
            _view = view;
            _sender = sender;
            InitActions();
        }

        private void InitActions()
        {
            _view.Saved += SavedClicked;
            _view.Canceled += CanceledClicked;
        }

        private void CanceledClicked()
        {
            ((AddEditPersonForm)_view).Close();
        }

        private void SavedClicked()
        {
            Person viewPerson = new Person
            {
                EmailAddress = _view.EmailTextBox.Text,
                Name = _view.NameTextBox.Text,
                Active = true
            };
            PersonService.Insert(viewPerson);
            CanceledClicked();
            _sender.PopulateEmailList();
        }
    }
}
