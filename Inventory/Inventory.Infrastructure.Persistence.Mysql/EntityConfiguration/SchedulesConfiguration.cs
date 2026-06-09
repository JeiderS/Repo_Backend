using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FleetManager.Domain.Schedules.Entity;

namespace FleetManager.Infrastructure.Persistence.Mysql.EntityConfiguration
{
    public class SchedulesConfiguration
    {
        public SchedulesConfiguration(EntityTypeBuilder<SchedulesEntity> builder)
        {
            builder.ToTable("schedules");

            builder.HasKey(s => s.Id);

            builder.Property(s => s.Id)
                .HasColumnName("id")
                .IsRequired();

            builder.Property(s => s.RouteId)
                .HasColumnName("route_id")
                .IsRequired();

            builder.Property(s => s.WeekNum)
                .HasColumnName("week_num")
                .IsRequired();

            builder.Property(s => s.FromDate)
                .HasColumnName("from_date")
                .IsRequired();

            builder.Property(s => s.ToDate)
                .HasColumnName("to_date")
                .IsRequired();

            builder.Property(s => s.DayOfWeek)
                .HasColumnName("day_of_week")
                .HasConversion<string>() // mapea el enum como string
                .IsRequired();
        }
    }
}
