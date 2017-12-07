using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.SqlServer.Utilities;
using System.Linq;
using System.Threading;
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
        private CancellationTokenSource _cancellationTokenSource;
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
            _view.FromDate.Value = _view.FromDate.Value.AddDays(-1);
            _view.RunButtonPressed += RunButtonClickActionAsync;
            _view.EnvComboBoxChanged += EnvComboBoxChangedAction;
            _view.DbComboBoxChanged += DbComboBoxChangedAction;
            _view.DgvCellDoubleClicked += DgvCellDoubleClickAction;
            _view.AddedPerson += AddPersonAction;
            _view.DeletedPerson += DeletePersonAction;
            _view.CreateButtonPressed += CreateReportButtonAction;
            _view.CancelButtonPressed += CancelButtonAction;
        }

        private void CancelButtonAction()
        {
            _cancellationTokenSource.Cancel();
        }

        private async void CreateReportButtonAction()
        {
            if (MessageBox.Show(@"Create Report?", @"Report Distribution", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                _view.ProgressBar.Visible = true;
                _view.AllErrorsGrid.DataSource = await BatchAuditService.AllErrors(_view, _connectionString);
                var grids = new List<DataGridView> {_view.DataGridView, _view.DataGridView2, _view.DataGridView3, _view.AllErrorsGrid };
                var env = _view.EnvComboBox.Text;
                var db = _view.DbComboBox.Text;
                var attachment =
                    await Task.Run(() => TableToExcelManager.ExportToExcel(grids, env, db));                   
                if (_view.EmailList.Rows.Count <= 0) return;
                _view.ProgressBar.Visible = false;
                if (MessageBox.Show(@"Send Report to distribution list?", @"Report Distribution",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    DistributionManager.SendReport(attachment, SelectedEnv, SelectedDb);
                }
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
            ClearGrids();
            _cancellationTokenSource = new CancellationTokenSource();           
            try
            {
                _view.ProgressBar.Visible = true;
                _view.RunButton.Visible = false;
                _view.CnclButton.Visible = true;

                var errorGroupsTask = BatchAuditService.GetErrorGroups(_view, _connectionString);
                var batchStatTask = BatchAuditService.BatchStatistics(_view, _connectionString);
                var taskListTask = BatchAuditService.TaskList(_view, _connectionString);
                var cancelTask = _cancellationTokenSource.Token.AsTask();
                var tasks = new List<Task<object>> {errorGroupsTask, batchStatTask, taskListTask, cancelTask};
                while (tasks.Count > 1)
                {
                    Task<object> taskRes = await Task.WhenAny(tasks);
                    if (taskRes == cancelTask)
                    {
                        throw new TaskCanceledException();
                    }
                    if (taskRes == errorGroupsTask)
                    {
                        _view.DataGridView.DataSource = await taskRes;
                    }
                    if (taskRes == batchStatTask)
                    {
                        _view.DataGridView3.DataSource = await taskRes;
                    }
                    if (taskRes == taskListTask)
                    {
                        _view.DataGridView2.DataSource = await taskRes;
                    }
                    tasks.Remove(taskRes);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, @"Cancelled", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                _view.ProgressBar.Visible = false;
                _view.RunButton.Visible = true;
                _view.CnclButton.Visible = false;
            }
            
        }

        private void ClearGrids()
        {
            _view.DataGridView.DataSource = null;
            _view.DataGridView2.DataSource = null;
            _view.DataGridView3.DataSource = null;
            _view.AllErrorsGrid.DataSource = null;
        }

        private void SetViewPropertiesFromModel()
        {
        }
    }
}