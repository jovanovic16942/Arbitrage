using Arbitrage.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Arbitrage.DataGetters
{
    public abstract class Parser : IParser
    {
        protected MatchesData _data;

        public MatchesData GetMatches(DateTime dateTime)
        {
            UpdateData(dateTime);

            return _data;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateTime"></param>
        protected abstract void UpdateData(DateTime dateTime);
    }
}
