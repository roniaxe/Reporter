﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Reporter.Model
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class alis_uatEntities : DbContext
    {
        public alis_uatEntities()
            : base("name=alis_uatEntities")
        {
        }
        public alis_uatEntities(string connString, string nameOrFull = "name=")
            : base($@"{nameOrFull}{connString}")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<g_batch_audit> g_batch_audit { get; set; }
        public virtual DbSet<t_batch> t_batch { get; set; }
        public virtual DbSet<t_task> t_task { get; set; }
        public virtual DbSet<sys_auth_data> sys_auth_data { get; set; }
    }
}
