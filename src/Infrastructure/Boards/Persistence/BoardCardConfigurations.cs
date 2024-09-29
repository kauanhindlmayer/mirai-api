using Domain.Boards;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Boards.Persistence;

public class BoardCardConfigurations : IEntityTypeConfiguration<BoardCard>
{
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
            .OnDelete(DeleteBehavior.NoAction);
    }
}