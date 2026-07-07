using Domain.Projects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

internal sealed class GitHubRepositoryConnectionConfigurations : IEntityTypeConfiguration<GitHubRepositoryConnection>
{
    public void Configure(EntityTypeBuilder<GitHubRepositoryConnection> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .ValueGeneratedNever();

        builder.Property(c => c.ProjectId)
            .IsRequired();

        builder.Property(c => c.InstallationId)
            .IsRequired();

        builder.Property(c => c.RepositoryId)
            .IsRequired();

        builder.Property(c => c.RepositoryOwner)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(c => c.RepositoryName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(c => c.ConnectedByUserId)
            .IsRequired();

        builder.Property(c => c.ConnectedAtUtc)
            .IsRequired();

        builder.HasIndex(c => c.ProjectId)
            .IsUnique();

        builder.HasIndex(c => c.RepositoryId)
            .IsUnique();
    }
}
