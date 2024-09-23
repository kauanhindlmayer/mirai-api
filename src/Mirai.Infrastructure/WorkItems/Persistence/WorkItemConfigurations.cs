using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mirai.Domain.WorkItems;
using Mirai.Domain.WorkItems.Enums;

namespace Mirai.Infrastructure.WorkItems.Persistence;

public class WorkItemConfigurations : IEntityTypeConfiguration<WorkItem>
{
    public void Configure(EntityTypeBuilder<WorkItem> builder)
    {
        builder.HasKey(wi => wi.Id);

        builder.Property(wi => wi.Id)
            .ValueGeneratedNever();

        builder.Property(wi => wi.Code)
            .IsRequired();

        builder.Property(wi => wi.Title)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(wi => wi.Description);

        builder.Property(wi => wi.Type)
            .HasConversion(
                v => v.Name,
                v => WorkItemType.FromName(v, false))
            .IsRequired();

        builder.Property(wi => wi.Status)
            .HasConversion(
                v => v.Name,
                v => WorkItemStatus.FromName(v, false))
            .IsRequired();

        builder.ComplexProperty(wi => wi.Planning);

        builder.ComplexProperty(wi => wi.Classification)
            .Property(wi => wi.ValueArea)
            .HasConversion(
                v => v.Name,
                v => ValueAreaType.FromName(v, false));

        builder.Property(wi => wi.AssigneeId);

        builder.Property(wi => wi.ProjectId)
            .IsRequired();

        builder.HasOne(wi => wi.Assignee)
            .WithMany(u => u.WorkItems)
            .HasForeignKey(wi => wi.AssigneeId);

        builder.HasOne(wi => wi.Project)
            .WithMany(wi => wi.WorkItems)
            .HasForeignKey(wi => wi.ProjectId);

        builder.HasOne(wi => wi.ParentWorkItem)
            .WithMany(wi => wi.ChildWorkItems)
            .HasForeignKey(wi => wi.ParentWorkItemId);

        builder.HasMany(wi => wi.ChildWorkItems)
            .WithOne(wi => wi.ParentWorkItem)
            .HasForeignKey(wi => wi.ParentWorkItemId);

        builder.HasMany(wi => wi.Comments)
            .WithOne(c => c.WorkItem)
            .HasForeignKey(c => c.WorkItemId);

        builder.HasMany(wi => wi.Tags)
            .WithMany(t => t.WorkItems)
            .UsingEntity(j => j.ToTable("WorkItemTag"));
    }
}