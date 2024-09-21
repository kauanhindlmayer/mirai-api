using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mirai.Domain.Retrospectives;

namespace Mirai.Infrastructure.Retrospectives.Persistence;

public class RetrospectiveItemConfigurations : IEntityTypeConfiguration<RetrospectiveItem>
{
    public void Configure(EntityTypeBuilder<RetrospectiveItem> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .ValueGeneratedNever();

        builder.Property(p => p.Description)
            .IsRequired();

        builder.Property(p => p.Votes)
            .IsRequired();

        builder.Property(p => p.RetrospectiveColumnId)
            .IsRequired();

        builder.HasOne(p => p.RetrospectiveColumn)
            .WithMany(o => o.Items)
            .HasForeignKey(p => p.RetrospectiveColumnId);

        builder.Property(p => p.AuthorId)
            .IsRequired();

        builder.HasOne(p => p.Author)
            .WithMany()
            .HasForeignKey(p => p.AuthorId);
    }
}