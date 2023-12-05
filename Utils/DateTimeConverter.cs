using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arbitrage.Utils
{
    public class DateTimeConverter
    {
        public static DateTime DateTimeFromLong(long dateTimeLong, int offsetHours = 0)
        {
            DateTime unixEpoch = new(1970, 1, 1, 0, 0, 0, DateTimeKind.Local);
            var dateTime = unixEpoch.AddMilliseconds(dateTimeLong).AddHours(offsetHours);
            return dateTime;
        }
    }
}
