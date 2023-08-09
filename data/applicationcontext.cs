using Microsoft.EntityFrameworkCore;
using new1.Models;

namespace new1.data
{
    public class applicationcontext : DbContext
    {
        public applicationcontext(DbContextOptions<applicationcontext> options) : base(options) { }

        public DbSet<register> register { get; set; }
        public DbSet<contact_us> contact_us { get; set; }
        public DbSet<home> home { get; set; }
    }
}
    