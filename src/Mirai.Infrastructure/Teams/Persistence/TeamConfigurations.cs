using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mirai.Domain.Teams;

namespace Mirai.Infrastructure.Teams.Persistence;

public class TeamConfigurations : IEntityTypeConfiguration<Team>
{
    public void Configure(EntityTypeBuilder<Team> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .ValueGeneratedNever();

        builder.Property(p => p.Name)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(p => p.ProjectId)
            .IsRequired();

        builder.HasOne(p => p.Project)
            .WithMany(o => o.Teams)
            .HasForeignKey(p => p.ProjectId);
    }
}