using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Zup.Entities;

public partial class tbl_TaskEntryTag
{
    public Guid TaskID { get; set; }
    public Guid TagID { get; set; }
}

public partial class tbl_TaskEntryTag : IEntityTypeConfiguration<tbl_TaskEntryTag>
{
    public void Configure(EntityTypeBuilder<tbl_TaskEntryTag> builder)
    {
        builder.HasKey(a => new
        {
            a.TaskID,
            a.TagID
        });

        builder.HasOne<tbl_Tag>()
            .WithMany()
            .HasForeignKey(a => a.TagID);

        builder.HasOne<tbl_TaskEntry>()
            .WithMany()
            .HasForeignKey(a => a.TaskID);
    }
}