using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arbitrage.EntityFramework.Models
{
    [Table(name: "Countries")]
    public class Country
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Code { get; set; } = null!;

        public List<League>? Leagues { get; set; }
        public List<Team>? Teams { get; set; }
    }
}
