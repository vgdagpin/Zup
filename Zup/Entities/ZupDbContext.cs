using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zup.Entities;

public class ZupDbContext : DbContext
{
    public DbSet<tbl_TimeLog> TimeLogs { get; set; }

    public ZupDbContext(DbContextOptions<ZupDbContext> dbContextOptions) : base(dbContextOptions)
    {
        
    }
}
