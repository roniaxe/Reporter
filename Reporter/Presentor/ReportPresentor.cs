using System;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Reporter.Model;
using Reporter.View;
using Reporter.Data.Services;
using Reporter.Forms;
using Reporter.Utils;
using Reporter.ViewModel;

namespace Reporter.Presentor
{
    internal class ReportPresentor
    {
        private readonly IReportView _view;
        private string _connectionString;


        public ReportPresentor(IReportView view)
        {
            _view = view;
            SetViewPropertiesFromModel();
            WireUpViewEvents();
            PopulateEmailList();
        }

        private string SelectedDb => _view.DbComboBox.SelectedItem.ToString();
        private string SelectedEnv => _view.EnvComboBox.SelectedItem.ToString();

        public void PopulateEmailList()
        {
            _view.PersonBindingSource.DataSource = null;
            _view.PersonBindingSource.DataSource = PersonService.GetAll();
        }

        private void WireUpViewEvents()
        {
            _view.RunButtonPressed += RunButtonClickActionAsync;
            _view.EnvComboBoxChanged += EnvComboBoxChangedAction;
            _view.DbComboBoxChanged += DbComboBoxChangedAction;
            _view.DgvCellDoubleClicked += DgvCellDoubleClickAction;
            _view.AddedPerson += AddPersonAction;
            _view.DeletedPerson += DeletePersonAction;
            _view.CreateButtonPressed += CreateReportButtonAction;
        }

        private void CreateReportButtonAction()
        {
            if (_view.EmailList.Rows.Count == 0)
            {
                MessageBox.Show(@"Empty Distribution List", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (MessageBox.Show(@"Send Report?", @"Report Distribution", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                var attachment = TableToExcelManager.ExportToExcel(_view.DataGridView, SelectedEnv, SelectedDb);
                DistributionManager.SendReport(attachment, SelectedEnv, SelectedDb);
            }            
        }

        private void DeletePersonAction()
        {
            PersonService.Remove(_view.PersonBindingSource.Current as Person);
            PopulateEmailList();
        }

        private void AddPersonAction()
        {
            AddEditPersonForm addEditForm = new AddEditPersonForm();
            new AddEditPersonPresentor(addEditForm, this);
            addEditForm.Show();
        }

        private void DgvCellDoubleClickAction(object sender, DataGridViewCellEventArgs e)
        {
            Console.WriteLine($@"Column {e.ColumnIndex}, Row {e.RowIndex}");
            if (_view.DataGridView.Columns[e.ColumnIndex].HeaderText.Equals("Message"))
            {
                CommentForm commentForm = new CommentForm();
                string ValueOfCellAtColumnIndex(int rowNum) => _view.DataGridView.Rows[e.RowIndex].Cells[rowNum].Value as string;
                ErrorCommentConn model = new ErrorCommentConn
                {
                    Comments = ValueOfCellAtColumnIndex(0),
                    ErrorMessage = ValueOfCellAtColumnIndex(1),
                    Batch = ValueOfCellAtColumnIndex(3),
                    Task = ValueOfCellAtColumnIndex(4)
                };
                var getFromDb = CommentService.GetById(model);
                new CommentPresentor(commentForm, getFromDb ?? model);
                commentForm.Show();
            }
        }

        private void DbComboBoxChangedAction()
        {
            BuildConnectionString();
        }

        private void EnvComboBoxChangedAction()
        {
            if (string.IsNullOrEmpty(SelectedEnv))
            {
                _view.DbComboBox.DataSource = null;
                return;
            }
            using (var db = new alis_uatEntities(SelectedEnv))
            {
                _view.DbComboBox.DataSource = db.sys_auth_data.Select(sad => sad.db_name).ToList();
            }
        }

        private void BuildConnectionString()
        {
            if (string.IsNullOrEmpty(SelectedEnv)) return;
            var connection = ConfigurationManager.ConnectionStrings[SelectedEnv].ConnectionString;
            var catalogStartPos = connection.IndexOf("initial catalog=", StringComparison.Ordinal) +
                                  "initial catalog=".Length;
            var catalogEndPos = connection.IndexOf(';', catalogStartPos);
            var oldCatalogName = connection.Substring(catalogStartPos, catalogEndPos - catalogStartPos);
            connection = connection.Replace(oldCatalogName, SelectedDb);
            _connectionString = connection;
        }

        private async void RunButtonClickActionAsync()
        {
            _view.ProgressBar.Visible = true;
            _view.DataGridView.DataSource = await Task.Run(() => BatchAuditService.GetErrorGroups(_view, _connectionString));
            _view.ProgressBar.Visible = false;
        }

        private void SetViewPropertiesFromModel()
        {
        }
    }
}