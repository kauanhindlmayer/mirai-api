using Domain.Boards;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Boards.Persistence;

public class BoardConfigurations : IEntityTypeConfiguration<Board>
{
    public void Configure(EntityTypeBuilder<Board> builder)
    {
        builder.HasKey(b => b.Id);

        builder.Property(b => b.Id)
            .ValueGeneratedNever();

        builder.Property(b => b.Name)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(b => b.Description)
            .HasMaxLength(500);

        builder.Property(b => b.ProjectId)
            .IsRequired();

        builder.HasOne(b => b.Project)
            .WithMany(o => o.Boards)
            .HasForeignKey(b => b.ProjectId);

        builder.HasMany(b => b.Columns)
            .WithOne(c => c.Board)
            .HasForeignKey(c => c.BoardId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}