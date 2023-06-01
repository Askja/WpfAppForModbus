namespace WpfAppForModBus.Domain;

public sealed class ApplicationContext : Microsoft.EntityFrameworkCore.DbContext {
    public ApplicationContext() {
        Database.EnsureCreated();
    }

    public Microsoft.EntityFrameworkCore.DbSet<WpfAppForModBus.Domain.Entities.Sensor> Sensors =>
        Set<WpfAppForModBus.Domain.Entities.Sensor>();

    public Microsoft.EntityFrameworkCore.DbSet<WpfAppForModBus.Domain.Entities.SensorsData> SensorsData =>
        Set<WpfAppForModBus.Domain.Entities.SensorsData>();

    protected override void OnConfiguring(Microsoft.EntityFrameworkCore.DbContextOptionsBuilder optionsBuilder) {
        Microsoft.EntityFrameworkCore.SqliteDbContextOptionsBuilderExtensions.UseSqlite(optionsBuilder: optionsBuilder,
            connectionString: "Data Source=SensorsData.db");
    }
}