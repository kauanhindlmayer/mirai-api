using Domain.WorkItems;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

internal sealed class WorkItemPullRequestLinkConfigurations : IEntityTypeConfiguration<WorkItemPullRequestLink>
{
    public void Configure(EntityTypeBuilder<WorkItemPullRequestLink> builder)
    {
        builder.HasKey(l => l.Id);

        builder.Property(l => l.Id)
            .ValueGeneratedNever();

        builder.Property(l => l.WorkItemId)
            .IsRequired();

        builder.Property(l => l.PullRequestId)
            .IsRequired();

        builder.Property(l => l.PullRequestNumber)
            .IsRequired();

        builder.Property(l => l.Title)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(l => l.HtmlUrl)
            .IsRequired()
            .HasMaxLength(2048);

        builder.Property(l => l.State)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(l => l.AuthorLogin)
            .HasMaxLength(255);

        builder.Property(l => l.Source)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(l => l.LinkedAtUtc)
            .IsRequired();

        builder.HasOne(l => l.WorkItem)
            .WithMany(wi => wi.PullRequestLinks)
            .HasForeignKey(l => l.WorkItemId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(l => new { l.WorkItemId, l.PullRequestId })
            .IsUnique();

        builder.HasIndex(l => l.PullRequestId);
    }
}
