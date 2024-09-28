using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mirai.Domain.WorkItems;

namespace Mirai.Infrastructure.WorkItems.Persistence;

public class WorkItemCommentConfigurations : IEntityTypeConfiguration<WorkItemComment>
{
    public void Configure(EntityTypeBuilder<WorkItemComment> builder)
    {
        builder.HasKey(wic => wic.Id);

        builder.Property(wic => wic.Id)
            .ValueGeneratedNever();

        builder.Property(wic => wic.WorkItemId)
            .IsRequired();

        builder.Property(wic => wic.UserId)
            .IsRequired();

        builder.Property(wic => wic.Content)
            .HasColumnType("NVARCHAR(MAX)")
            .IsRequired();
    }
}