using Domain.WikiPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

internal sealed class WikiPageViewConfigurations : IEntityTypeConfiguration<WikiPageView>
{
    public void Configure(EntityTypeBuilder<WikiPageView> builder)
    {
        builder.HasKey(wpv => wpv.Id);

        builder.Property(wpv => wpv.Id)
            .ValueGeneratedNever();

        builder.Property(wpv => wpv.WikiPageId)
            .IsRequired();

        builder.Property(wpv => wpv.ViewedAt)
            .IsRequired();

        builder.Property(wpv => wpv.ViewerId)
            .IsRequired();
    }
}