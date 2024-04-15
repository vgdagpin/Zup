using Microsoft.EntityFrameworkCore;
using Zup.Entities;

namespace Zup;

public class ZupDbContext : DbContext
{
    public DbSet<tbl_TimeLog> TimeLogs { get; set; }
    public DbSet<tbl_Note> Notes { get; set; }

    public ZupDbContext(DbContextOptions<ZupDbContext> dbContextOptions) : base(dbContextOptions)
    {

    }
}
