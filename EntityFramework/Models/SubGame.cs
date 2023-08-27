using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arbitrage.EntityFramework.Models
{
    [Table(name: "SubGames")]
    public class SubGame
    {
        [Key]
        public int Id { get; set; }
        public int? MozzartID { get; set; }
        public string? Description { get; set; }
        public string? Name { get; set; }
        public int GameId { get; set; }

        public Game? Game { get; set; } = null!;
    }
}
