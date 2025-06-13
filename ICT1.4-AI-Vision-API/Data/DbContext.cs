using ICT1._4_AI_Vision_API.Models;
using Microsoft.EntityFrameworkCore;

namespace ICT1._4_AI_Vision_API.Data
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
                .HasOne(l => l.Weather)
                .WithOne(w => w.Litter)
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
