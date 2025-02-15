using Domain.Retrospectives;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

internal sealed class RetrospectiveItemConfigurations : IEntityTypeConfiguration<RetrospectiveItem>
{
    public void Configure(EntityTypeBuilder<RetrospectiveItem> builder)
    {
        builder.HasKey(ri => ri.Id);

        builder.Property(ri => ri.Id)
            .ValueGeneratedNever();

        builder.Property(ri => ri.Content)
            .IsRequired();

        builder.Property(ri => ri.Votes)
            .IsRequired();

        builder.Property(ri => ri.RetrospectiveColumnId)
            .IsRequired();

        builder.HasOne(ri => ri.RetrospectiveColumn)
            .WithMany(o => o.Items)
            .HasForeignKey(ri => ri.RetrospectiveColumnId);

        builder.Property(ri => ri.AuthorId)
            .IsRequired();

        builder.HasOne(ri => ri.Author)
            .WithMany()
            .HasForeignKey(ri => ri.AuthorId);
    }
}