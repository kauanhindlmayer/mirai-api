using Domain.Boards;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

internal sealed class BoardConfigurations :
    IEntityTypeConfiguration<Board>,
    IEntityTypeConfiguration<BoardColumn>,
    IEntityTypeConfiguration<BoardCard>
{
    public void Configure(EntityTypeBuilder<Board> builder)
    {
        builder.HasKey(b => b.Id);

        builder.Property(b => b.Id)
            .ValueGeneratedNever();

        builder.Property(b => b.Name)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(b => b.TeamId)
            .IsRequired();

        builder.HasMany(b => b.Columns)
            .WithOne(c => c.Board)
            .HasForeignKey(c => c.BoardId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    public void Configure(EntityTypeBuilder<BoardColumn> builder)
    {
        builder.HasKey(bc => bc.Id);

        builder.Property(bc => bc.Id)
            .ValueGeneratedNever();

        builder.Property(bc => bc.Name)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(bc => bc.Position)
            .IsRequired();

        builder.Property(bc => bc.WipLimit)
            .IsRequired(false);

        builder.Property(bc => bc.DefinitionOfDone)
            .HasMaxLength(1000)
            .IsRequired(false);

        builder.Property(bc => bc.BoardId)
            .IsRequired();

        builder.HasOne(bc => bc.Board)
            .WithMany(o => o.Columns)
            .HasForeignKey(bc => bc.BoardId);

        builder.HasMany(bc => bc.Cards)
            .WithOne(c => c.BoardColumn)
            .HasForeignKey(c => c.BoardColumnId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    public void Configure(EntityTypeBuilder<BoardCard> builder)
    {
        builder.HasKey(bc => bc.Id);

        builder.Property(bc => bc.Id)
            .ValueGeneratedNever();

        builder.Property(bc => bc.BoardColumnId)
            .IsRequired();

        builder.Property(bc => bc.WorkItemId)
            .IsRequired();

        builder.Property(bc => bc.Position)
            .IsRequired();

        builder.HasOne(bc => bc.BoardColumn)
            .WithMany(o => o.Cards)
            .HasForeignKey(bc => bc.BoardColumnId);

        builder.HasOne(bc => bc.WorkItem)
            .WithMany()
            .HasForeignKey(bc => bc.WorkItemId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}