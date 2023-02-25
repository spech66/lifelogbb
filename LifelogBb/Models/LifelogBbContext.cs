using LifelogBb.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace LifelogBb.Models
{
    public class LifelogBbContext : DbContext
    {
        public DbSet<Weight> Weights { get; set; } = null!;
        public DbSet<StrengthTraining> StrengthTrainings { get; set; } = null!;
        public DbSet<EnduranceTraining> EnduranceTrainings { get; set; } = null!;
        public DbSet<Journal> Journals { get; set; } = null!;
        public DbSet<BucketList> BucketLists { get; set; } = null!;
        public DbSet<Quote> Quotes { get; set; } = null!;
        public DbSet<Todo> Todos { get; set; } = null!;
        public DbSet<Habit> Habits { get; set; } = null!;
        public DbSet<Goal> Goals { get; set; } = null!;

        private readonly IConfiguration _configuration;

        private IDbContextTransaction? _transaction = null;

        public string DbPath { get; }

        public LifelogBbContext(IConfiguration configuration)
        {
            _configuration = configuration;

            var path = _configuration["Database:Path"];
            if (string.IsNullOrEmpty(path))
            {
                var folder = Environment.SpecialFolder.LocalApplicationData;
                path = Environment.GetFolderPath(folder);
                path = Path.Join(path, "lifelogbb");
            }
            if (!Path.Exists(path))
            {
                Directory.CreateDirectory(path);
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
                if (_transaction != null)
                {
                    _transaction.Commit();
                }
            }
            finally
            {
                if (_transaction != null)
                {
                    _transaction.Dispose();
                }
            }
        }

        public void Rollback()
        {
            if (_transaction != null)
            {
                _transaction.Rollback();
                _transaction.Dispose();
            }
        }
    }
}
