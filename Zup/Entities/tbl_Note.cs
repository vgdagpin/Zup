using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using System.ComponentModel.DataAnnotations;

namespace Zup.Entities;

public partial class tbl_Note
{
    public int ID { get; set; }

    public int LogID { get; set; }

    [MaxLength(1000)]
    public string Notes { get; set; } = null!;

    public DateTime CreatedOn { get; set; }

    public DateTime? UpdatedOn { get; set; }
}

public partial class tbl_Note : IEntityTypeConfiguration<tbl_Note>
{
    public void Configure(EntityTypeBuilder<tbl_Note> builder)
    {
        builder.HasOne<tbl_TimeLog>()
            .WithMany()
            .HasForeignKey(a => a.LogID);
    }
}