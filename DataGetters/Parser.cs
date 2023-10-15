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

        public MatchesData Parse()
        {
            UpdateData();

            return _data;
        }

        /// <summary>
        /// 
        /// </summary>
        protected abstract void UpdateData();
    }
}
