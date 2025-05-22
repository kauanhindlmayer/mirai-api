using Domain.WorkItems;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

internal sealed class WorkItemConfigurations : IEntityTypeConfiguration<WorkItem>
{
    public void Configure(EntityTypeBuilder<WorkItem> builder)
    {
        builder.HasKey(wi => wi.Id);

        builder.Property(wi => wi.Id)
            .ValueGeneratedNever();

        builder.HasIndex(wi => new { wi.ProjectId, wi.Code })
            .IsUnique();

        builder.Property(wi => wi.Code)
            .IsRequired();

        builder.Property(wi => wi.Title)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(wi => wi.Description);

        builder.Property(wi => wi.AcceptanceCriteria);

        builder.Property(wi => wi.Type)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(wi => wi.Status)
            .HasConversion<string>()
            .IsRequired();

        builder.ComplexProperty(wi => wi.Planning);

        builder.ComplexProperty(wi => wi.Classification)
            .Property(wi => wi.ValueArea)
            .HasConversion<string>();

        builder.Property(wi => wi.SearchVector)
            .HasColumnType("vector")
            .IsRequired();

        builder.Property(wi => wi.AssigneeId);

        builder.Property(wi => wi.ProjectId)
            .IsRequired();

        builder.Property(wi => wi.AssignedTeamId)
            .IsRequired(false);

        builder.HasOne(wi => wi.Assignee)
            .WithMany(u => u.WorkItems)
            .HasForeignKey(wi => wi.AssigneeId);

        builder.HasOne(wi => wi.Project)
            .WithMany(wi => wi.WorkItems)
            .HasForeignKey(wi => wi.ProjectId);

        builder.HasOne(wi => wi.AssignedTeam)
            .WithMany(t => t.WorkItems)
            .HasForeignKey(wi => wi.AssignedTeamId);

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
            .UsingEntity(j => j.ToTable("work_item_tags"));

        builder.Property(wi => wi.CompletedAtUtc);

        builder.HasOne(wi => wi.Sprint)
            .WithMany(s => s.WorkItems)
            .HasForeignKey(wi => wi.SprintId);
    }
}