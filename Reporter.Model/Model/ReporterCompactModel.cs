using Microsoft.EntityFrameworkCore;
using Reporter.Model.Entities;

namespace Reporter.Model.Model
{
    public class ReporterCompactModel : DbContext
    {
        public ReporterCompactModel(DbContextOptions<ReporterCompactModel> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite();
        }

        public virtual DbSet<Person> Persons { get; set; }
    }
}
