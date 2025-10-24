using Domain.WorkItems;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

internal sealed class WorkItemConfigurations :
    IEntityTypeConfiguration<WorkItem>,
    IEntityTypeConfiguration<WorkItemComment>,
    IEntityTypeConfiguration<WorkItemAttachment>,
    IEntityTypeConfiguration<WorkItemLink>
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

        builder.HasMany(wi => wi.Attachments)
            .WithOne(a => a.WorkItem)
            .HasForeignKey(a => a.WorkItemId);

        builder.HasMany(wi => wi.Tags)
            .WithMany(t => t.WorkItems)
            .UsingEntity(j => j.ToTable("work_item_tags"));

        builder.HasMany(wi => wi.OutgoingLinks)
            .WithOne(wil => wil.SourceWorkItem)
            .HasForeignKey(wil => wil.SourceWorkItemId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(wi => wi.IncomingLinks)
            .WithOne(wil => wil.TargetWorkItem)
            .HasForeignKey(wil => wil.TargetWorkItemId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(wi => wi.CompletedAtUtc);

        builder.HasOne(wi => wi.Sprint)
            .WithMany(s => s.WorkItems)
            .HasForeignKey(wi => wi.SprintId);

        builder.HasIndex(wi => wi.AssigneeId);
        builder.HasIndex(wi => wi.AssignedTeamId);
        builder.HasIndex(wi => wi.SprintId);
        builder.HasIndex(wi => wi.ParentWorkItemId);
        builder.HasIndex(wi => wi.Status);
        builder.HasIndex(wi => wi.Type);
    }

    public void Configure(EntityTypeBuilder<WorkItemComment> builder)
    {
        builder.HasKey(wic => wic.Id);

        builder.Property(wic => wic.Id)
            .ValueGeneratedNever();

        builder.Property(wic => wic.WorkItemId)
            .IsRequired();

        builder.Property(p => p.AuthorId)
            .IsRequired();

        builder.Property(wic => wic.Content)
            .IsRequired();

        builder.HasOne(p => p.Author)
            .WithMany()
            .HasForeignKey(p => p.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    public void Configure(EntityTypeBuilder<WorkItemAttachment> builder)
    {
        builder.HasKey(wia => wia.Id);

        builder.Property(wia => wia.Id)
            .ValueGeneratedNever();

        builder.Property(wia => wia.WorkItemId)
            .IsRequired();

        builder.Property(wia => wia.FileName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(wia => wia.BlobId)
            .IsRequired();

        builder.Property(wia => wia.ContentType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(wia => wia.FileSizeBytes)
            .IsRequired();

        builder.Property(wia => wia.AuthorId)
            .IsRequired();

        builder.HasIndex(wia => wia.WorkItemId);
        builder.HasIndex(wia => wia.BlobId);
    }

    public void Configure(EntityTypeBuilder<WorkItemLink> builder)
    {
        builder.HasKey(wil => wil.Id);

        builder.Property(wil => wil.Id)
            .ValueGeneratedNever();

        builder.Property(wil => wil.SourceWorkItemId)
            .IsRequired();

        builder.Property(wil => wil.TargetWorkItemId)
            .IsRequired();

        builder.Property(wil => wil.LinkType)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(wil => wil.Comment)
            .HasMaxLength(500);

        builder.HasIndex(wil => wil.SourceWorkItemId);
        builder.HasIndex(wil => wil.TargetWorkItemId);
        builder.HasIndex(wil => wil.LinkType);

        builder.HasIndex(wil => new { wil.SourceWorkItemId, wil.TargetWorkItemId, wil.LinkType })
            .IsUnique();
    }
}