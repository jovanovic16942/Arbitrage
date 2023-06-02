using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arbitrage.EntityFramework.Models
{
    [Table(name: "Games")]
    public class Game
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string ShortName { get; set; } = null!;

        public List<SubGame>? SubGames { get; set; }
    }
}
