using Domain.Entities.Schedules;
using Domain.Entities.Schedules.ValueObjects;
using Domain.Entities.Users;
using Domain.Entities.Users.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configs;


public class ChecklistStateConfig : IEntityTypeConfiguration<ChecklistState>
{
    public void Configure(EntityTypeBuilder<ChecklistState> builder)
    {
        // keeping the owner tracked by EF
        builder.Ignore(x => x.Owner);

        // ---------- PRIMARY KEY ----------

        builder.Property(x => x.Id)
               .HasConversion(
                   v => v.Value,
                   v => new ChecklistId(v)
               );
        builder.HasKey(x => x.Id);

        // ---------- REQUIRED FIELDS ----------

        builder.Property(x => x.UserId)
               .HasConversion(
                   v => v.Value,
                   v => new UserId(v)
               );
        builder.HasOne<UserState>()
               .WithMany()
               .HasForeignKey(x => x.UserId)
               .IsRequired();

        builder.HasMany(x => x.Tasks)
               .WithOne()
               .HasForeignKey(x => x.ChecklistId)
               .IsRequired();

        builder.OwnsOne(x => x.Statistics, sb =>
        {
            sb.Property(s => s.CreatedAt).IsRequired();

            sb.OwnsOne(s => s.UserRating, ur =>
            {
                ur.Property(r => r.MoodRating).HasColumnName("User_MoodRating");
                ur.Property(r => r.MotivationRating).HasColumnName("User_MotivationRating");
                ur.Property(r => r.EffortRating).HasColumnName("User_EffortRating");
                ur.Property(r => r.ProductivityRating).HasColumnName("User_ProductivityRating");
                ur.Property(r => r.FocusRating).HasColumnName("User_FocusRating");
                ur.Property(r => r.StressRating).HasColumnName("User_StressRating");
            });

            sb.OwnsOne(s => s.LLMRating, llm =>
            {
                llm.Property(r => r.MoodRating).HasColumnName("LLM_MoodRating");
                llm.Property(r => r.MotivationRating).HasColumnName("LLM_MotivationRating");
                llm.Property(r => r.EffortRating).HasColumnName("LLM_EffortRating");
                llm.Property(r => r.ProductivityRating).HasColumnName("LLM_ProductivityRating");
                llm.Property(r => r.FocusRating).HasColumnName("LLM_FocusRating");
                llm.Property(r => r.StressRating).HasColumnName("LLM_StressRating");
            });
        });
    }
}

