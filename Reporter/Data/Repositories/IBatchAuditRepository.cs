using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Reporter.Data.Services;
using Reporter.Model;
using Reporter.Utils;
using Reporter.ViewModel;
using Reporter.ViewModel.ServiceModel;

namespace Reporter.Data.Repositories
{
    internal interface IBatchAuditRepository : IGenericRepository<g_batch_audit>
    {
        Task<object> GetErrorGroups((DateTime FromDate, DateTime ToDate) serviceModel);
        Task<object> GetBatchStatistics((DateTime FromDate, DateTime ToDate) serviceModel);
        Task<object> GetTaskList((DateTime FromDate, DateTime ToDate) serviceModel);
        Task<object> GetAllErrors((DateTime FromDate, DateTime ToDate) serviceModel);
        Task<object> GetAuditByPolicy(AuditByPolicyServiceModel serviceModel);
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

        public async Task<object> GetErrorGroups((DateTime FromDate, DateTime ToDate) serviceModel)
        {
            using (var db = new alis_uatEntities())
            {
                var q = await (
                    from gBatchAudit in db.g_batch_audit
                    where gBatchAudit.entry_time >= serviceModel.FromDate &&
                          gBatchAudit.entry_time < serviceModel.ToDate &&
                          (gBatchAudit.entry_type == 5 || gBatchAudit.entry_type == 6)
                    join tTask in db.t_task on gBatchAudit.task_id equals tTask.task_id into firstJoin
                    from tTask in firstJoin.DefaultIfEmpty()
                    join tBatch in db.t_batch on gBatchAudit.batch_id equals tBatch.batch_id
                    select new
                    {
                        BatchAudit = gBatchAudit,
                        Task = tTask.task_name,
                        Batch = tBatch.batch_name
                    }).ToListAsync();
                
                q.ForEach(rec =>
                    rec.BatchAudit.description = Regex.Replace(rec.BatchAudit.description, @"[\d-]", string.Empty));

                var newQ =
                    from rec in q
                    group rec by new {rec.BatchAudit.description, rec.Batch, rec.Task, rec.BatchAudit.batch_run_num}
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

                return newQ
                    .OrderBy(grp => grp.BatchRunNumber)
                    .ThenBy(grp => grp.Count).ToSbl();
            }
        }

        public async Task<object> GetBatchStatistics((DateTime FromDate, DateTime ToDate) serviceModel)
        {
            using (var db = new alis_uatEntities())
            {
                var q = from gba in db.g_batch_audit
                    where gba.entry_time > serviceModel.FromDate && 
                    gba.entry_time < serviceModel.ToDate && 
                    (gba.description.Contains("completed , reached") || gba.batch_id == 168 && gba.entry_type == 3)
                    join tTask in db.t_task on gba.task_id equals tTask.task_id
                    join tBatch in db.t_batch on gba.batch_id equals tBatch.batch_id
                    group gba by new {tTask.task_name, tBatch.batch_name, gba.batch_run_num}
                    into grouped
                    select new
                    {
                        grouped.Key.batch_run_num,
                        grouped.Key.batch_name,
                        grouped.Key.task_name,
                        //TotalTime = DbFunctions.DiffMinutes(grouped.Min(g => g.entry_time),
                        Chunked = grouped.Sum(g => g.chunk_id) > 1 ? "Yes" : "No",
                        Proccessed = grouped.Count()
                    };
                return await q.OrderBy(stat => stat.batch_run_num).ToListAsync();
            }
        }

        public async Task<object> GetTaskList((DateTime FromDate, DateTime ToDate) serviceModel)
        {
            using (var db = new alis_uatEntities())
            {
                var q =
                    from gba in db.g_batch_audit
                    join tTask in db.t_task on gba.task_id equals tTask.task_id
                    join tBatch in db.t_batch on gba.batch_id equals tBatch.batch_id
                    where gba.entry_time > serviceModel.FromDate && gba.entry_time < serviceModel.ToDate
                    group new { gba, tTask, tBatch } by new
                    {
                        gba.batch_run_num,
                        tTask.task_id,
                        tBatch.batch_id,
                        tTask.task_name,
                        tBatch.batch_name
                    }
                    into g
                    orderby g.Min(p => p.gba.entry_time)
                    select new
                    {
                        g.Key.batch_name,
                        g.Key.batch_id,
                        g.Key.task_name,
                        g.Key.task_id,
                        g.Key.batch_run_num,
                        StartTime = g.Min(p => p.gba.entry_time)
                        //g.Key.batch_id,                        
                        //g.Key.task_id

                    };
                return await q.ToListAsync();
            }
        }

        public async Task<object> GetAllErrors((DateTime FromDate, DateTime ToDate) serviceModel)
        {
            using (var db = new alis_uatEntities())
            {
                var q = from gba in db.g_batch_audit
                    join tt in db.t_task on gba.task_id equals tt.task_id
                    join tb in db.t_batch on gba.batch_id equals tb.batch_id
                    where
                        gba.entry_time > serviceModel.FromDate &&
                        gba.entry_time < serviceModel.ToDate &&
                        (gba.entry_type == 6 || gba.entry_type == 5)
                    orderby new { gba.description}
                    select new
                    {
                        Message = gba.description,
                        EntityType = gba.primary_key_type,
                        EntityId = gba.primary_key,
                        Batch = tb.batch_name,
                        BatchId = tb.batch_id,
                        Task = tt.task_name,
                        TaskId = tt.task_id,
                        BatchRunNumber = gba.batch_run_num
                    };
                return await q
                     .OrderBy(errs => errs.BatchRunNumber)
                    .ThenBy(errs => errs.Message)
                    .ToListAsync();
            }
        }

        public async Task<object> GetAuditByPolicy(AuditByPolicyServiceModel serviceModel)
        {
            using (var db = new alis_uatEntities())
            {
                var q = from gba in db.g_batch_audit
                    join tt in db.t_task on gba.task_id equals tt.task_id
                    join tb in db.t_batch on gba.batch_id equals tb.batch_id
                    where
                        gba.entry_time > serviceModel.FromDate &&
                        gba.entry_time < serviceModel.ToDate &&
                        (string.IsNullOrEmpty(serviceModel.PolicyNo) || gba.primary_key.Equals(serviceModel.PolicyNo)) &&
                        (string.IsNullOrEmpty(serviceModel.ExternalPolicyNo) || gba.primary_key.Equals(serviceModel.ExternalPolicyNo)) &&
                        (string.IsNullOrEmpty(serviceModel.ClientNo) || gba.primary_key.Equals(serviceModel.ClientNo)) 
                    select new
                    {
                        MessageType = gba.entry_type == 6 || gba.entry_type == 5 ? "Error" : "Process",
                        Message = gba.description,
                        EntityType = gba.primary_key_type,
                        EntityId = gba.primary_key,
                        Batch = tb.batch_name,
                        BatchId = tb.batch_id,
                        Task = tt.task_name,
                        TaskId = tt.task_id,
                        BatchRunNumber = gba.batch_run_num
                    };
                return await q.ToListAsync();
            }
        }
    }
}
