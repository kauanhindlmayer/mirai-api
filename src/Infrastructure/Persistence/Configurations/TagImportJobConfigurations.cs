using Domain.TagImportJobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

internal sealed class TagImportJobConfigurations : IEntityTypeConfiguration<TagImportJob>
{
  public void Configure(EntityTypeBuilder<TagImportJob> builder)
  {
    builder.HasKey(e => e.Id);

    builder.Property(t => t.Id)
      .ValueGeneratedNever();

    builder.Property(e => e.FileName)
        .HasMaxLength(500)
        .IsRequired();
  }
}