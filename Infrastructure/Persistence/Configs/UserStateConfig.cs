using Domain.Entities.Users;
using Domain.Entities.Users.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configs;


public class UserStateConfig : IEntityTypeConfiguration<UserState>
{
    public void Configure(EntityTypeBuilder<UserState> builder)
    {
        builder.Property(s => s.Owner)
            .HasConversion(
                    o => o.GetHashCode(),
                    o => null!
                    );

        builder.Property(u => u.Id)
                .HasConversion(
                        v => v.Value,
                        v => new UserId(v)
                    );
        builder.HasKey(u => u.Id);

        builder.OwnsOne(x => x.Name,
                nb => nb.Property(n => n.Value).HasColumnName("Name"));

        builder.OwnsOne(x => x.PasswordHash,
                pb => pb.Property(p => p.Value).HasColumnName("PasswordHash"));
    }
}

