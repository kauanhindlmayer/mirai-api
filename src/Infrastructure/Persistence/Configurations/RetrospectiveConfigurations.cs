using Domain.Retrospectives;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

internal sealed class RetrospectiveConfigurations :
    IEntityTypeConfiguration<Retrospective>,
    IEntityTypeConfiguration<RetrospectiveColumn>,
    IEntityTypeConfiguration<RetrospectiveItem>
{
    public void Configure(EntityTypeBuilder<Retrospective> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
            .ValueGeneratedNever();

        builder.Property(r => r.Title)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(r => r.MaxVotesPerUser)
            .IsRequired();

        builder.Property(r => r.Template)
            .IsRequired();

        builder.Property(r => r.TeamId)
            .IsRequired();

        builder.HasOne(r => r.Team)
            .WithMany(o => o.Retrospectives)
            .HasForeignKey(r => r.TeamId);

        builder.HasMany(r => r.Columns)
            .WithOne(c => c.Retrospective)
            .HasForeignKey(c => c.RetrospectiveId);

        builder.HasIndex(r => r.TeamId);
    }

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

        builder.Property(ri => ri.AuthorId)
            .IsRequired();

        builder.HasOne(ri => ri.RetrospectiveColumn)
            .WithMany(o => o.Items)
            .HasForeignKey(ri => ri.RetrospectiveColumnId);

        builder.HasOne(ri => ri.Author)
            .WithMany()
            .HasForeignKey(ri => ri.AuthorId);
    }
}