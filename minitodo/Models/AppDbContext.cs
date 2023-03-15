using Microsoft.EntityFrameworkCore;

namespace minitodo.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }


    }
}
