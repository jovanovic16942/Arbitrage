using Arbitrage.EntityFramework.Models;
using Microsoft.EntityFrameworkCore;

namespace Arbitrage.EntityFramework
{
    public class ArbitrageDbContext : DbContext
    {
        public ArbitrageDbContext(DbContextOptions<ArbitrageDbContext> options) : base(options) { }
        
        public DbSet<Game> Games { get; set; }
    }
}
