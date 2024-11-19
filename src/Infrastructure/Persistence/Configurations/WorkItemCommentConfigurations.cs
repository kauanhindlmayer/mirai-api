using Domain.WorkItems;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

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
            .IsRequired();
    }
}