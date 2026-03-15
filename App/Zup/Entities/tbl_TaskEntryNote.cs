using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations;

namespace Zup.Entities;

public partial class tbl_TaskEntryNote
{
    public Guid ID { get; set; }

    public Guid TaskID { get; set; }

    [MaxLength(1000)]
    public string Notes { get; set; } = null!;

    public string RTF { get; set; } = null!;

    public DateTime CreatedOn { get; set; }

    public DateTime? UpdatedOn { get; set; }
}

public partial class tbl_TaskEntryNote : IEntityTypeConfiguration<tbl_TaskEntryNote>
{
    public void Configure(EntityTypeBuilder<tbl_TaskEntryNote> builder)
    {
        builder.HasOne<tbl_TaskEntry>()
            .WithMany()
            .HasForeignKey(a => a.TaskID);
    }
}