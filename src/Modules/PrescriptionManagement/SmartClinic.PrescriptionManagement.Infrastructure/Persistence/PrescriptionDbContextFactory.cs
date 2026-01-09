using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SmartClinic.PrescriptionManagement.Infrastructure.Persistence
{
    // Design-time factory for EF Core tools to create the DbContext when running migrations.
    public class PrescriptionDbContextFactory : IDesignTimeDbContextFactory<PrescriptionDbContext>
    {
        public PrescriptionDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<PrescriptionDbContext>();

            // Allow override from environment variable; otherwise use localhost postgres (matches docker-compose)
            var conn = System.Environment.GetEnvironmentVariable("POSTGRES_CONNECTION")
                       ?? "Host=localhost;Port=5432;Database=clinic_db;Username=clinic_user;Password=clinic_password";

            builder.UseNpgsql(conn);

            return new PrescriptionDbContext(builder.Options);
        }
    }
}
