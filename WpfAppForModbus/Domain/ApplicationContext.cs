using Microsoft.EntityFrameworkCore;
using WpfAppForModbus.Domain.Entities;

namespace WpfAppForModbus.Domain {
    public class ApplicationContext : DbContext {
        public DbSet<Sensor> Sensors => Set<Sensor>();
        public DbSet<SensorsData> SensorsData => Set<SensorsData>();

        public ApplicationContext() => Database.EnsureCreated();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.UseSqlite("Data Source=SensorsData.db");
        }
    }
}
