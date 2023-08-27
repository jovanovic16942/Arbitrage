using Microsoft.EntityFrameworkCore;
using Arbitrage.EntityFramework.Models;

namespace Arbitrage.EntityFramework
{

    // TODO Check before insert if the value 
    public class ArbitrageDb
    {
        private static ArbitrageDb _instance = new ArbitrageDb();
        public static ArbitrageDb Instance()
        {
            return _instance;
        }

        private ArbitrageDb() {

            var arbitrageDbOptions = new DbContextOptionsBuilder<ArbitrageDbContext>()
                .UseSqlServer("Server=(localdb)\\Local;initial catalog=Arbitrage;" +
                "Integrated Security=true;MultipleActiveResultSets=true;App=Arbitrage")
                .Options;

            Context = new ArbitrageDbContext(arbitrageDbOptions);
            _countries = Context.Countries.ToList();
            _teams = Context.Teams.ToList();
        }

        public void InsertGame(Game game)
        {
            InsertEntity(game);
        }

        public void InsertGames(IEnumerable<Game> games)
        {
            InsertEntityList(games);
        }

        public void InsertSubGame(SubGame subGame)
        {
            InsertEntity(subGame);
        }

        public void InsertSubGames(IEnumerable<SubGame> subGames)
        {
            InsertEntityList(subGames);
        }

        public void InsertTeam(Team team)
        {
            
            if(ShouldInsertTeam(team))
            {
                _teams.Add(team);
                InsertEntity(team);
            }
        }

        public void InsertTeams(IEnumerable<Team> teams)
        {
            teams = teams.Where(team => ShouldInsertTeam(team));            
            InsertEntityList(teams);
            _teams.AddRange(teams);
        }

        private bool ShouldInsertTeam(Team team)
        {
            return _teams.FirstOrDefault(t => t.Name.Trim().ToLower() == team.Name.Trim().ToLower()) == null
                && _countries.FirstOrDefault(x => x.Name.Trim().ToLower() == team.Name.Trim().ToLower()) == null;
        }

        public void InsertCountry(Country country)
        {
            InsertEntity(country);
        }

        public void InsertContries(IEnumerable<Country> countries)
        {
            InsertEntityList(countries);
        }

        public void InsertLeague(League league)
        {
            InsertEntity(league);
        }

        public void InsertLeagues(IEnumerable<League> leagues)
        {
            InsertEntityList(leagues);
        }

        private void InsertEntity(dynamic entityObject)
        {
            Context.Add(entityObject);
            Context.SaveChanges();
        }

        private void InsertEntityList(IEnumerable<dynamic> entities)
        {
            foreach(dynamic entity in entities)
            {
                Context.Add(entity);
            }
            Context.SaveChanges();
        }


        public readonly ArbitrageDbContext Context;
        private List<Team> _teams;
        private List<Country> _countries;
    }
}
