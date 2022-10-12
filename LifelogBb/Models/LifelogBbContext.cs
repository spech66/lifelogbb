using LifelogBb.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace LifelogBb.Models
{
    public class LifelogBbContext : DbContext
    {
        public DbSet<Weight> Weights { get; set; }
        public DbSet<StrengthTraining> StrengthTrainings { get; set; }
        public DbSet<EnduranceTraining> EnduranceTrainings { get; set; }
        public DbSet<Journal> Journals { get; set; }

        private readonly IConfiguration Configuration;

        private IDbContextTransaction _transaction;

        public string DbPath { get; }

        public LifelogBbContext(IConfiguration configuration)
        {
            Configuration = configuration;

            var path = Configuration["Database:Path"];
            if(string.IsNullOrEmpty(path))
            {
                var folder = Environment.SpecialFolder.LocalApplicationData;
                path = Environment.GetFolderPath(folder);
            }
            DbPath = Path.Join(path, "liefelogbb.db");
        }

        // Create Sqlite database file in the "local" folder.
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source={DbPath}");

        public void BeginTransaction()
        {
            _transaction = Database.BeginTransaction();
        }

        public void Commit()
        {
            try
            {
                SaveChanges();
                _transaction.Commit();
            }
            finally
            {
                _transaction.Dispose();
            }
        }

        public void Rollback()
        {
            _transaction.Rollback();
            _transaction.Dispose();
        }
    }
}
