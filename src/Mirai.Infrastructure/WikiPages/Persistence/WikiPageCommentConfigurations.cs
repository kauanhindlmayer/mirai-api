using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mirai.Domain.WikiPages;

namespace Mirai.Infrastructure.WikiPages.Persistence;

public class WikiPageCommentConfigurations : IEntityTypeConfiguration<WikiPageComment>
{
    public void Configure(EntityTypeBuilder<WikiPageComment> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .ValueGeneratedNever();

        builder.Property(p => p.WikiPageId)
            .IsRequired();

        builder.Property(p => p.UserId)
            .IsRequired();

        builder.Property(p => p.Content)
            .IsRequired();
    }
}