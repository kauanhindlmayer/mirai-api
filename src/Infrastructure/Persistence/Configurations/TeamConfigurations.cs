using Domain.Teams;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class TeamConfigurations : IEntityTypeConfiguration<Team>
{
    public void Configure(EntityTypeBuilder<Team> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id)
            .ValueGeneratedNever();

        builder.Property(t => t.Name)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(t => t.ProjectId)
            .IsRequired();

        builder.HasOne(t => t.Project)
            .WithMany(o => o.Teams)
            .HasForeignKey(t => t.ProjectId);
    }
}