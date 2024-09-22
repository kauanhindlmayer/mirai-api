using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mirai.Domain.WorkItems;

namespace Mirai.Infrastructure.WorkItems.Persistence;

public class WorkItemConfigurations : IEntityTypeConfiguration<WorkItem>
{
    public void Configure(EntityTypeBuilder<WorkItem> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .ValueGeneratedNever();

        builder.Property(p => p.Title)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Description);

        builder.Property(p => p.Type)
            .HasConversion(
                v => v.Name,
                v => WorkItemType.FromName(v, false))
            .IsRequired();

        builder.Property(p => p.Status)
            .HasConversion(
                v => v.Name,
                v => WorkItemStatus.FromName(v, false))
            .IsRequired();

        builder.Property(p => p.AssigneeId);

        builder.Property(p => p.ProjectId)
            .IsRequired();

        builder.HasOne(p => p.Assignee)
            .WithMany(u => u.WorkItems)
            .HasForeignKey(p => p.AssigneeId);

        builder.HasOne(p => p.Project)
            .WithMany(p => p.WorkItems)
            .HasForeignKey(p => p.ProjectId);

        builder.HasOne(p => p.ParentWorkItem)
            .WithMany(p => p.ChildWorkItems)
            .HasForeignKey(p => p.ParentWorkItemId);

        builder.HasMany(p => p.ChildWorkItems)
            .WithOne(p => p.ParentWorkItem)
            .HasForeignKey(p => p.ParentWorkItemId);

        builder.HasMany(p => p.Comments)
            .WithOne(c => c.WorkItem)
            .HasForeignKey(c => c.WorkItemId);

        builder.HasMany(p => p.Tags)
            .WithMany(t => t.WorkItems)
            .UsingEntity(j => j.ToTable("WorkItemTag"));
    }
}