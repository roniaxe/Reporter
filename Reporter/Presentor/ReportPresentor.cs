using System;
using System.Linq;
using System.Text.RegularExpressions;
using Reporter.Model;
using Reporter.View;
using System.Configuration;
using Reporter.DataLayer;
using Reporter.Utils;

namespace Reporter.Presentor
{
    internal class ReportPresentor
    {
        private readonly IReportView _view;
        private string _connectionString;
        private string SelectedDb => _view.DbComboBox.SelectedItem.ToString();
        private string SelectedEnv => _view.EnvComboBox.SelectedItem.ToString();

        public ReportPresentor(IReportView view)
        {
            _view = view;
            SetViewPropertiesFromModel();
            WireUpViewEvents();
            PopulateEmailList();
        }

        private void PopulateEmailList()
        {
            _view.EmailList.DataSource = PersonService.GetAll();
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
            if (string.IsNullOrEmpty(SelectedEnv)) return;
            using (var db = new alis_uatEntities(SelectedEnv))
            {
                _view.DbComboBox.DataSource = db.sys_auth_data.Select(sad => sad.db_name).ToList();
            }
        }

        private void BuildConnectionString()
        {
            var connection = ConfigurationManager.ConnectionStrings[SelectedEnv].ConnectionString;
            var catalogPos = connection.IndexOf("initial catalog=", StringComparison.Ordinal) + "initial catalog=".Length;
            var catalogPosEnd = connection.IndexOf(';', catalogPos);
            var oldCatalogName = connection.Substring(catalogPos, catalogPosEnd-catalogPos);
            connection = connection.Replace(oldCatalogName, SelectedDb);
            //var existing = new EntityConnectionStringBuilder(connection);
            //var builder = new System.Data.SqlClient.SqlConnectionStringBuilder(existing.ProviderConnectionString)
            //    {
            //        ["Initial Catalog"] = SelectedDb
            //};
            //existing.ProviderConnectionString = builder.ConnectionString;           
            //existing.ConnectionString = existing.ConnectionString.Replace("Application Name", "App");
            _connectionString = connection;
        }

        private void CreateButtonClickAction()
        {
            using (var db = new alis_uatEntities(_connectionString))
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
                        Date = groupedQuery.Min(d => d.BatchAudit.entry_time),
                        groupedQuery.Key.Batch,
                        groupedQuery.Key.Task,
                        BatchRunNumber = groupedQuery.Key.batch_run_num,
                        Count = groupedQuery.Count()
                    };

                var sortedList = newQ.ToSbl();
                _view.DataGridView.DataSource = sortedList;
            }
        }

        private void SetViewPropertiesFromModel()
        {
        }
    }
}