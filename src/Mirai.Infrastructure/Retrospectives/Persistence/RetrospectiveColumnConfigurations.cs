using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mirai.Domain.Retrospectives;

namespace Mirai.Infrastructure.Retrospectives.Persistence;

public class RetrospectiveColumnConfigurations : IEntityTypeConfiguration<RetrospectiveColumn>
{
    public void Configure(EntityTypeBuilder<RetrospectiveColumn> builder)
    {
        builder.HasKey(rc => rc.Id);

        builder.Property(rc => rc.Id)
            .ValueGeneratedNever();

        builder.Property(rc => rc.Title)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(rc => rc.RetrospectiveId)
            .IsRequired();

        builder.HasOne(rc => rc.Retrospective)
            .WithMany(o => o.Columns)
            .HasForeignKey(rc => rc.RetrospectiveId);
    }
}