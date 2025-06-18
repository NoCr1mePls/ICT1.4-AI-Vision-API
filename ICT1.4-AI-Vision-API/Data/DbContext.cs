using SensoringApi.Models;
using Microsoft.EntityFrameworkCore;

namespace SensoringApi.Data
{
    public class LitterDbContext : DbContext
    {
        public LitterDbContext(DbContextOptions<LitterDbContext> options) : base(options)
        {
        }

        public DbSet<Litter> Litter { get; set; }
        public DbSet<Weather> Weather { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Litter>()
                .HasOne(l => l.weather)
				.WithOne(w => w.litter)
				.HasForeignKey<Weather>(w => w.weather_id);
			modelBuilder.Entity<Litter>()
                .Property(l => l.litter_id)
                .ValueGeneratedNever();

            modelBuilder.Entity<Weather>()
                .Property(w => w.weather_id)
                .ValueGeneratedNever(); 
        }
    }
}
