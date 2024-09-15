using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mirai.Domain.WikiPages;

namespace Mirai.Infrastructure.WikiPages.Persistence;

public class WikiPageConfigurations : IEntityTypeConfiguration<WikiPage>
{
    public void Configure(EntityTypeBuilder<WikiPage> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .ValueGeneratedNever();

        builder.Property(p => p.ProjectId)
            .IsRequired();

        builder.Property(p => p.Title)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(p => p.Content)
            .IsRequired();

        builder.HasMany(p => p.SubWikiPages)
            .WithOne(p => p.ParentWikiPage)
            .HasForeignKey(p => p.ParentWikiPageId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(p => p.Comments)
            .WithOne(c => c.WikiPage)
            .HasForeignKey(c => c.WikiPageId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}