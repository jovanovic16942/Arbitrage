using Arbitrage.DataGetters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arbitrage.DataLoader
{
    internal class DataLoader
    {
        MozzartData _data;

        MozzartParser _parser = new MozzartParser();

        public DataLoader() {}

        public List<Match> GetMatches(DateTime? dateTime) {

            dateTime ??= DateTime.Now;

            UpdateData(dateTime.Value);

            return _data.GetMatches().Where(x => x.StartTime.Date == dateTime.Value.Date).ToList();
        }

        private void UpdateData(DateTime dateTime)
        {
            _data = _parser.GetData(dateTime);

            // TODO update database
        }
    }
}
