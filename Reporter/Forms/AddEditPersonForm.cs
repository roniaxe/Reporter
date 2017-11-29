using System;
using System.Windows.Forms;
using Reporter.View;

namespace Reporter.Forms
{
    public partial class AddEditPersonForm : Form, IAddEditPersonView
    {
        public event Action Saved;
        public event Action Canceled;

        public AddEditPersonForm()
        {
            InitializeComponent();
            btnSave.Click += SavedClicked;
            btnCancel.Click += CanceledClicked;
        }

        private void CanceledClicked(object sender, EventArgs e)
        {
            Canceled?.Invoke();
        }

        private void SavedClicked(object sender, EventArgs e)
        {
            Saved?.Invoke();
        }

        public TextBox NameTextBox
        {
            get => txtName;
            set => txtName = value;
        }

        public TextBox EmailTextBox
        {
            get => txtEmail;
            set => txtEmail = value;
        }
    }
}
