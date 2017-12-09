﻿using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Reporter.Data.Services;
using Reporter.Model;
using Reporter.Utils;
using Reporter.View;
using Reporter.ViewModel;

namespace Reporter.Data.Repositories
{
    internal interface IBatchAuditRepository : IGenericRepository<g_batch_audit>
    {
        Task<object> GetErrorGroups(IReportView view, string connString);
        Task<object> GetBatchStatistics(IReportView view, string connString);
        Task<object> GetTaskList(IReportView view, string connString);
        Task<object> GetAllErrors(IReportView view, string connString);
        Task<object> GetAuditByPolicy(IPolicyFilterView view, string connString, string policyNo);
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

        public async Task<object> GetErrorGroups(IReportView view, string connString)
        {
            using (var db = new alis_uatEntities(connString))
            {
                var q = await (from gBatchAudit in db.g_batch_audit
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

                return newQ.ToSbl();
            }
        }

        public async Task<object> GetBatchStatistics(IReportView view, string connString)
        {
            using (var db = new alis_uatEntities(connString))
            {
                var q = from gba in db.g_batch_audit
                    where gba.entry_time > view.FromDate.Value.Date && 
                    gba.entry_time < view.ToDate.Value.Date && 
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
                            //grouped.Max(g => g.entry_time)),
                        //TotalTime = (grouped.Max(g => g.entry_time) - grouped.Min(g => g.entry_time)).TotalMinutes.ToString("F"),
                        Chunked = grouped.Sum(g => g.chunk_id) > 1 ? "Yes" : "No",
                        Proccessed = grouped.Count()
                    };
                return await q.ToListAsync();
            }
        }

        public async Task<object> GetTaskList(IReportView view, string connString)
        {
            using (var db = new alis_uatEntities(connString))
            {
                var q =
                    from gba in db.g_batch_audit
                    join tTask in db.t_task on gba.task_id equals tTask.task_id
                    join tBatch in db.t_batch on gba.batch_id equals tBatch.batch_id
                    where gba.entry_time > view.FromDate.Value.Date && gba.entry_time < view.ToDate.Value.Date
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
                        g.Key.task_name,
                        g.Key.batch_run_num,
                        StartTime = g.Min(p => p.gba.entry_time)
                        //g.Key.batch_id,                        
                        //g.Key.task_id

                    };
                return await q.ToListAsync();
            }
        }

        public async Task<object> GetAllErrors(IReportView view, string connString)
        {
            using (var db = new alis_uatEntities(connString))
            {
                var q = from gba in db.g_batch_audit
                    join tt in db.t_task on gba.task_id equals tt.task_id
                    join tb in db.t_batch on gba.batch_id equals tb.batch_id
                    where
                        gba.entry_time > view.FromDate.Value.Date &&
                        gba.entry_time < view.ToDate.Value.Date &&
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
                return await q.ToListAsync();
            }
        }

        public async Task<object> GetAuditByPolicy(IPolicyFilterView view, string connString, string policyNo)
        {
            using (var db = new alis_uatEntities(connString))
            {
                var q = from gba in db.g_batch_audit
                    join tt in db.t_task on gba.task_id equals tt.task_id
                    join tb in db.t_batch on gba.batch_id equals tb.batch_id
                    where
                        gba.entry_time > view.FromDate.Value.Date &&
                        gba.entry_time < view.ToDate.Value.Date &&
                        (string.IsNullOrEmpty(view.PolicyNo.Text) || gba.primary_key.Equals(view.PolicyNo.Text)) &&
                        (string.IsNullOrEmpty(view.ExtPolicyNo.Text) || gba.primary_key.Equals(view.ExtPolicyNo.Text)) &&
                        (string.IsNullOrEmpty(view.ClientNo.Text) || gba.primary_key.Equals(view.ClientNo.Text)) 
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
