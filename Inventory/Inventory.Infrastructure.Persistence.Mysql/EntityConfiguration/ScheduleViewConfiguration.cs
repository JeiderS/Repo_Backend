using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FleetManager.Domain.ScheduleView.Entity;

namespace FleetManager.Infrastructure.Persistence.Mysql.EntityConfiguration
{
    public class ScheduleViewConfiguration
    {
        public ScheduleViewConfiguration(EntityTypeBuilder<ScheduleViewEntity> builder)
        {
            builder.ToView("schedule_view");

            builder.HasNoKey();

            builder.Property(sv => sv.Day)
                .HasColumnName("dia")
                .HasMaxLength(20);

            builder.Property(sv => sv.Route)
                .HasColumnName("ruta")
                .HasMaxLength(255);

            builder.Property(sv => sv.Origin)
                .HasColumnName("origen")
                .HasMaxLength(255);

            builder.Property(sv => sv.Destination)
                .HasColumnName("destino")
                .HasMaxLength(255);

            builder.Property(sv => sv.StartTime)
                .HasColumnName("hora_inicio")
                .HasMaxLength(10);

            builder.Property(sv => sv.EndTime)
                .HasColumnName("hora_fin")
                .HasMaxLength(10);

            builder.Property(sv => sv.DriverName)
                .HasColumnName("conductor")
                .HasMaxLength(150);

            builder.Property(sv => sv.VehicleDescription)
                .HasColumnName("vehiculo")
                .HasMaxLength(255);
        }
    }
}
