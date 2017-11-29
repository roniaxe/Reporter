using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using Reporter.Data.Services;
using Reporter.Model;
using Reporter.Utils;
using Reporter.View;
using Reporter.ViewModel;

namespace Reporter.Data.Repositories
{
    internal interface IBatchAuditRepository : IGenericRepository<g_batch_audit>
    {
        object GetErrorGroups(IReportView view, string connString);
    }

    internal class BatchAuditRepository : IBatchAuditRepository
    {
        public List<g_batch_audit> GetAll()
        {
            using (var db = new alis_uatEntities())
            {
                return db.g_batch_audit.ToList();
            }
        }

        public g_batch_audit GetById(object obj)
        {
            using (var db = new alis_uatEntities())
            {
                return db.g_batch_audit.FirstOrDefault(gba => gba.pk == (int) obj);
            }
        }

        public g_batch_audit Insert(g_batch_audit obj)
        {
            using (var db = new alis_uatEntities())
            {
                db.g_batch_audit.Add(obj);
                db.SaveChanges();
                return obj;
            }
        }

        public void Update(g_batch_audit obj)
        {
            using (var db = new alis_uatEntities())
            {
                db.g_batch_audit.Attach(obj);
                db.Entry(obj).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        public object GetErrorGroups(IReportView view, string connString)
        {
            using (var db = new alis_uatEntities(connString))
            {
                var q = (from gBatchAudit in db.g_batch_audit
                    where gBatchAudit.entry_time >= view.FromDate.Value.Date &&
                          gBatchAudit.entry_time < view.ToDate.Value.Date &&
                          (gBatchAudit.entry_type == 5 || gBatchAudit.entry_type == 6)
                    join tTask in db.t_task on gBatchAudit.task_id equals tTask.task_id into firstJoin
                    from tTask in firstJoin.DefaultIfEmpty()
                    join tBatch in db.t_batch on gBatchAudit.batch_id equals tBatch.batch_id
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
                        Comment = CommentService.GetById(new ErrorCommentConn
                        {
                            ErrorMessage = groupedQuery.Key.description,
                            Batch = groupedQuery.Key.Batch,
                            Task = groupedQuery.Key.Task
                        })?.Comments,
                        Message = groupedQuery.Key.description,
                        Date = groupedQuery.Min(d => d.BatchAudit.entry_time),
                        groupedQuery.Key.Batch,
                        groupedQuery.Key.Task,
                        BatchRunNumber = groupedQuery.Key.batch_run_num,
                        Count = groupedQuery.Count()
                    };

                var sortedList = newQ.ToSbl();
                return sortedList;
            }
        }
    }
}
