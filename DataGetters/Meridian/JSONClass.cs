using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arbitrage.DataGetters.Meridian
{
    public class JsonMatchResponse
    {
        public List<JsonEvent> events { get; set; }
    }

    public class JsonEvent
    {
        public string id { get; set; }
        public int leagueId { get; set; }
        public string leagueName { get; set; }
        public string name { get; set; }
        public List<JsonTeam> team { get; set; }
        public string startTime { get; set; }
        public List<JsonSelection> standardShortMarkets { get; set; }
        public List<JsonSelection> market { get; set; }

        public int sportId;

    }

    public class JsonTeam
    {
        public string id { get; set; }
        public string name { get; set; }
    }


    public class JsonMatch
    {
        public List<JsonMarket> MarketShort { get; set; }
    }

    public class JsonNameTranslation
    {
        public string locale { get; set; }
        public string translation { get; set; }
    }

    public class JsonSelectionItem
    {
        public string name { get; set; }
        public string price { get; set; }
        public string state { get; set; }
        public string activationPrice { get; set; }
        public List<JsonNameTranslation> nameTranslations { get; set; }
    }

    public class JsonSelection
    {
        public List<JsonSelectionItem> selection { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public string templateId { get; set; }
        public string templateName { get; set; }
        public string state { get; set; }
        public DateTime creationTime { get; set; }
        public DateTime activationTime { get; set; }
        public double winnerPrice { get; set; }
        public List<JsonNameTranslation> nameTranslations { get; set; }
        public int inc { get; set; }
        public string overUnder { get; set; }
        public object handicap { get; set; }
        public List<JsonSelectionItems> selections { get; set; } // Dodao sam ovo polje za treći objekat u listi
    }

    public class JsonSelectionItems
    {
        public List<JsonSelectionItem> selection { get; set; }
    }

    public class JsonMarket
    {
        public List<JsonSelection> Selection { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public string TemplateId { get; set; }
        public string TemplateName { get; set; }
        public string State { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime ActivationTime { get; set; }
        public string WinnerPrice { get; set; }
        public List<JsonNameTranslation> NameTranslations { get; set; }
        public int Inc { get; set; }
        public string OverUnder { get; set; }
        public string ActivationOverUnder { get; set; }
        public string Handicap { get; set; }
    }

}
