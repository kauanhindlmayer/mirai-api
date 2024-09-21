using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mirai.Domain.Retrospectives;

namespace Mirai.Infrastructure.Retrospectives.Persistence;

public class RetrospectiveColumnConfigurations : IEntityTypeConfiguration<RetrospectiveColumn>
{
    public void Configure(EntityTypeBuilder<RetrospectiveColumn> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .ValueGeneratedNever();

        builder.Property(p => p.Title)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(p => p.RetrospectiveId)
            .IsRequired();

        builder.HasOne(p => p.Retrospective)
            .WithMany(o => o.Columns)
            .HasForeignKey(p => p.RetrospectiveId);
    }
}