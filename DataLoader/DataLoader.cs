﻿using Arbitrage.DataGetters;
using Arbitrage.Utils;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arbitrage.DataLoader
{
    internal class DataLoader
    {
        MatchesData? _data;

        IParser _parser;
        readonly string _name;

        public DataLoader(IParser parser)
        {
            _parser = parser;
            _name = _parser.GetName();
        }

        public MatchesData? GetMatches()
        {
            return _data;
        }

        public async Task Load() {
            Console.WriteLine(_name + " download started...");

            await UpdateData();
            Console.WriteLine(_name + " download complete");
        }

        private async Task UpdateData()
        {
            _data = _parser.Parse();

            // TODO update database
        }
    }
}
