using Domain.WorkItems;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

internal sealed class WorkItemCommentConfigurations : IEntityTypeConfiguration<WorkItemComment>
{
    public void Configure(EntityTypeBuilder<WorkItemComment> builder)
    {
        builder.HasKey(wic => wic.Id);

        builder.Property(wic => wic.Id)
            .ValueGeneratedNever();

        builder.Property(wic => wic.WorkItemId)
            .IsRequired();

        builder.Property(p => p.AuthorId)
            .IsRequired();

        builder.HasOne(p => p.Author)
            .WithMany()
            .HasForeignKey(p => p.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(wic => wic.Content)
            .IsRequired();
    }
}