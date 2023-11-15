using a.Models;
using Microsoft.EntityFrameworkCore;

namespace a.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);
    }
    public DbSet<Employeese> Employee { get; set; }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<Employeese>()
            .HasData(
                new Employeese { Id = 1, UserName = "Johnson Williams", BirthDate = new DateTime(1951, 02, 04).ToUniversalTime(), Wage = 15000, DateOfEmployment = new DateTime(2023, 03, 03).ToUniversalTime(), Department = "FullStack" },
                new Employeese { Id = 2, UserName = "Garcia Rodriguez", BirthDate = new DateTime(2002, 01, 02).ToUniversalTime(), Wage = 10000, Department = "Enginere", DateOfEmployment = new DateTime(2020, 10, 02).ToUniversalTime()},
                new Employeese { Id = 3, UserName = "Wilson", BirthDate = new DateTime(1990, 11, 1).ToUniversalTime(), Wage = 8000, DateOfEmployment = new DateTime(2016, 04, 02).ToUniversalTime(), Department = "Back-end" }
            );
    }

}