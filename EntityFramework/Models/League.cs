using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arbitrage.EntityFramework.Models
{
    [Table(name: "Leagues")]
    public class League
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
        public int CountryId { get; set; }

        public Country? Country { get; set; } = null!;
        public List<Team>? Teams { get; set; }
    }
}
