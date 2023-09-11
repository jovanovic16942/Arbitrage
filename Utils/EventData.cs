using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arbitrage.Utils
{
    public class EventData
    {
        DateTime startTime;

        List<Participant> teams = new List<Participant>();

        List<HouseOdds> odds = new List<HouseOdds>();
    }
}
