using Microsoft.EntityFrameworkCore;
using Arbitrage.EntityFramework;
using Arbitrage.EntityFramework.Models;

namespace Arbitrage.EntityFramework
{
    public class ArbitrageDb
    {
        public ArbitrageDb() {

            var arbitrageDbOptions = new DbContextOptionsBuilder<ArbitrageDbContext>()
                .UseSqlServer("Server=(localdb)\\Local;initial catalog=Arbitrage;" +
                "Integrated Security=true;MultipleActiveResultSets=true;App=Arbitrage")
                .Options;

            Context = new ArbitrageDbContext(arbitrageDbOptions);
        }

        public readonly ArbitrageDbContext Context;
    }
}
