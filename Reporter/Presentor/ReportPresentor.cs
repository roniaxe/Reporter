using System;
using System.Linq;
using System.Text.RegularExpressions;
using Reporter.Model;
using Reporter.View;
using System.Configuration;
using System.Data.Entity.Core.EntityClient;

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
        }

        private void WireUpViewEvents()
        {
            _view.CreateButtonPressed += CreateButtonClickAction;
            _view.EnvComboBoxChanged += EnvComboBoxChangedAction;
            _view.DbComboBoxChanged += DbComboBoxChangedAction;
        }

        private void DbComboBoxChangedAction()
        {
            BuildConnectionString();
        }

        private void EnvComboBoxChangedAction()
        {
            //_view.DbComboBox.DataSource = null;
            if (string.IsNullOrEmpty(_view.EnvComboBox.SelectedItem.ToString())) return;
            using (var db = new alis_uatEntities(_view.EnvComboBox.SelectedItem.ToString()))
            {
                _view.DbComboBox.DataSource = db.sys_auth_data.Select(sad => sad.db_name).ToList();
            }
        }

        private void BuildConnectionString()
        {
            var connection = ConfigurationManager.ConnectionStrings[_view.EnvComboBox.SelectedItem.ToString()].ConnectionString;           
            EntityConnectionStringBuilder existing = new EntityConnectionStringBuilder(connection);
            System.Data.SqlClient.SqlConnectionStringBuilder builder =
                new System.Data.SqlClient.SqlConnectionStringBuilder(existing.ProviderConnectionString)
                {
                    ["Initial Catalog"] = _view.DbComboBox.SelectedItem.ToString()
                };
            existing.ProviderConnectionString = builder.ConnectionString;           
            existing.ConnectionString = existing.ConnectionString.Replace("Application Name", "App");
            Console.WriteLine(connection);
            Console.WriteLine(existing.ConnectionString);
            _connectionString = existing.ConnectionString;
        }

        private void CreateButtonClickAction()
        {
            using (var db = new alis_uatEntities(_connectionString,""))
            {
                var q = (from gBatchAudit in db.g_batch_audit
                         where gBatchAudit.entry_time >= _view.FromDate.Value &&
                               gBatchAudit.entry_time < _view.ToDate.Value &&
                               (gBatchAudit.entry_type == 5 || gBatchAudit.entry_type == 6)
                         join tTask in db.t_task on gBatchAudit.task_id equals tTask.task_id into firstJoin
                         from tTask in firstJoin.DefaultIfEmpty()
                         join tBatch in db.t_batch on gBatchAudit.batch_id equals tBatch.batch_id
                         //orderby gBatchAudit.entry_time descending 
                         select new
                         {
                             BatchAudit = gBatchAudit,
                             Task = tTask.task_name,
                             Batch = tBatch.batch_name
                         }).ToList();

                q.ForEach(rec =>
                    rec.BatchAudit.description = Regex.Replace(rec.BatchAudit.description, @"[\d-]", string.Empty));

                var newQ =
                    from rec in q
                    group rec by new { rec.BatchAudit.description, rec.Batch, rec.Task, rec.BatchAudit.batch_run_num }
                    into groupedQuery
                    select new
                    {
                        Message = groupedQuery.Key.description,
                        groupedQuery.Key.Batch,
                        groupedQuery.Key.Task,
                        BatchRunNumber = groupedQuery.Key.batch_run_num,
                        Count = groupedQuery.Count()
                    };

                _view.DataGridView.DataSource = newQ.ToList();
            }
        }

        private void SetViewPropertiesFromModel()
        {
        }
    }
}