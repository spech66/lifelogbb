using Microsoft.EntityFrameworkCore;

namespace LifelogBb.Modelst
{
    public class LifelogBbContext : DbContext
    {
        private readonly IConfiguration Configuration;

        public string DbPath { get; }

        public LifelogBbContext(IConfiguration configuration)
        {
            Configuration = configuration;

            var path = Configuration["Account:Password"];
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
    }
}
