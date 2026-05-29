using Microsoft.EntityFrameworkCore;

namespace HolidayApp.Database;

public class HolidaysDbContext(DbContextOptions<HolidaysDbContext> options) : DbContext(options)
{
    public DbSet<Holiday> Holiday { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("holidays");

        modelBuilder.Entity<Holiday>(entity =>
        {
            entity.ToTable("Holiday");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.CountryCode)
                .IsRequired()
                .HasMaxLength(2)
                .IsFixedLength();

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.LocalName)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.Date)
                .IsRequired();

            entity.HasIndex(e => new { e.CountryCode, e.Date })
                .HasDatabaseName("IX_Holiday_CountryCode_Date");
            
            entity.HasIndex(e => new { e.Date, e.CountryCode, e.Name })
                .IsUnique()
                .HasDatabaseName("IX_Holiday_Date_CountryCode_Name");
        });
    }
}