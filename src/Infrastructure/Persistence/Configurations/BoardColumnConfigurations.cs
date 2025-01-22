using Domain.Boards;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

internal sealed class BoardColumnConfigurations : IEntityTypeConfiguration<BoardColumn>
{
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
            .HasMaxLength(255)
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
}