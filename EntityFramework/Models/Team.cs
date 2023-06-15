using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arbitrage.EntityFramework.Models
{
    [Table(name: "Teams")]
    public class Team
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string ShortName { get; set; } = null!;
        public int? LeagueID { get; set; }
        public League? League { get; set; }
    }
}
