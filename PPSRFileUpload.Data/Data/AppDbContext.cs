using Microsoft.EntityFrameworkCore;
namespace Data.Model
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<FileConfig> FileConfig { get; set; }
        public DbSet<ImportedFiles> ImportedFiles { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<FileConfig>(e =>
            {
                e.HasKey(x => x.Id);
                e.HasIndex(e => e.Vin).IsUnique();
                e.HasIndex(e => e.SpgAcn).IsUnique().IsClustered(false);
                e.ToTable("FileConfigDetails");
            });
            builder.Entity<ImportedFiles>(e =>
            {
                e.HasKey(x => x.Id);
                e.HasIndex(e => e.FileName).IsUnique();
                e.ToTable("ImportedFileNames");
            });
        }
    }
}