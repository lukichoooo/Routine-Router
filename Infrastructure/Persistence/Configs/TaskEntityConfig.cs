using Domain.Entities.Schedules;
using Domain.Entities.Schedules.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configs;


public class TaskEntityConfig : IEntityTypeConfiguration<TaskEntity>
{
    public void Configure(EntityTypeBuilder<TaskEntity> builder)
    {
        builder.Property(t => t.Id)
               .HasConversion(
                    v => v.Value,
                    v => new TaskId(v)
                );
        builder.HasKey(t => t.Id);

        builder.OwnsOne(x => x.Name,
                nb => nb.Property(n => n.Value).HasColumnName("Name"));

        builder.OwnsOne(x => x.PlannedSchedule,
                nb =>
                {
                    nb.Property(n => n.StartTime).HasColumnName("PlannedSchedule_Start");
                    nb.Property(n => n.EndTime).HasColumnName("PlannedSchedule_End");
                });

        builder.OwnsOne(x => x.ActualSchedule,
                nb =>
                {
                    nb.Property(n => n.StartTime).HasColumnName("ActualSchedule_Start");
                    nb.Property(n => n.EndTime).HasColumnName("ActualSchedule_End");
                });

        builder.OwnsOne(x => x.TaskType,
                nb =>
                {
                    nb.Property(n => n.Name).HasColumnName("TaskType_Name");
                    nb.Property(n => n.Category).HasColumnName("TaskType_Category");
                });
    }
}

