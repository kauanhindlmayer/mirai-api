using Domain.WikiPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class WikiPageCommentConfigurations : IEntityTypeConfiguration<WikiPageComment>
{
    public void Configure(EntityTypeBuilder<WikiPageComment> builder)
    {
        builder.HasKey(wpc => wpc.Id);

        builder.Property(wpc => wpc.Id)
            .ValueGeneratedNever();

        builder.Property(wpc => wpc.WikiPageId)
            .IsRequired();

        builder.Property(wpc => wpc.UserId)
            .IsRequired();

        builder.Property(wpc => wpc.Content)
            .IsRequired();
    }
}