namespace Reporter.Model
{
    using System.Data.Entity;

    public class ReporterCompactModel : DbContext
    {
        public ReporterCompactModel()
            : base("name=ReporterCompactModel")
        {
        }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
           
        }
        
        public virtual DbSet<Person> Persons { get; set; }
    }
}
