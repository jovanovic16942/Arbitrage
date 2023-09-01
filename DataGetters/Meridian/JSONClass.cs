using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arbitrage.DataGetters.Meridian
{
    public class MatchResponse
    {
        public List<Event> events { get; set; }
    }

    public class Event
    {
        public int leagueId { get; set; }
        public string name { get; set; }
        public List<Team> team { get; set; }
        public string startTime { get; set; }
        public List<Selection> standardShortMarkets { get; set; }

    }

    public class Team
    {
        public string id { get; set; }
        public string name { get; set; }
    }


    public class Match
    {
        public List<Market> MarketShort { get; set; }
    }

    public class NameTranslation
    {
        public string locale { get; set; }
        public string translation { get; set; }
    }

    public class SelectionItem
    {
        public string name { get; set; }
        public string price { get; set; }
        public string state { get; set; }
        public string activationPrice { get; set; }
        public List<NameTranslation> nameTranslations { get; set; }
    }

    public class Selection
    {
        public List<SelectionItem> selection { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public string templateId { get; set; }
        public string templateName { get; set; }
        public string state { get; set; }
        public DateTime creationTime { get; set; }
        public DateTime activationTime { get; set; }
        public double winnerPrice { get; set; }
        public List<NameTranslation> nameTranslations { get; set; }
        public int inc { get; set; }
        public string overUnder { get; set; }
        public object handicap { get; set; }
        public List<SelectionItems> selections { get; set; } // Dodao sam ovo polje za treći objekat u listi
    }

    public class SelectionItems
    {
        public List<SelectionItem> selection { get; set; }
    }

    public class Market
    {
        public List<Selection> Selection { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public string TemplateId { get; set; }
        public string TemplateName { get; set; }
        public string State { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime ActivationTime { get; set; }
        public string WinnerPrice { get; set; }
        public List<NameTranslation> NameTranslations { get; set; }
        public int Inc { get; set; }
        public string OverUnder { get; set; }
        public string ActivationOverUnder { get; set; }
        public string Handicap { get; set; }
    }

}
