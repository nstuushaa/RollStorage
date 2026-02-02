using Microsoft.EntityFrameworkCore;

namespace RollStorage.Models
{
    public class RollContext: DbContext
    {
        public RollContext(DbContextOptions<RollContext> options)
            : base(options) 
        { 
        }

        public DbSet<Roll> Rolls { get; set; } = null;
    }
}
