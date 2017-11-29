using System.Data.Entity;
using Reporter.ViewModel;

namespace Reporter.Model
{
    public class ReporterCompactModel : DbContext
    {
        public ReporterCompactModel()
            : base("name=ReporterCompactModel")
        {
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<ReporterCompactModel>());
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer<ReporterCompactModel>(null);
            base.OnModelCreating(modelBuilder);
        }

        public virtual DbSet<Person> Persons { get; set; }
        public virtual DbSet<ErrorCommentConn> Comment { get; set; }
    }
}
