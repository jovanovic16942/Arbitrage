using Arbitrage.EntityFramework.Models;
using Arbitrage.General;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Arbitrage.DataGetters.BalkanBet
{
    public class BalkanBetGetter
    {
        public JsonMatchIdsResponse GetMatchIds()
        {
            string currentTime = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");

            string url = "https://sports-sm-distribution-api.de-2.nsoftcdn.com/api/v1/events?deliveryPlatformId=3&dataFormat=%7B%22default%22:%22object%22,%22events%22:%22array%22,%22outcomes%22:%22array%22%7D&language=%7B%22default%22:%22sr-Latn%22,%22events%22:%22sr-Latn%22,%22sport%22:%22sr-Latn%22,%22category%22:%22sr-Latn%22,%22tournament%22:%22sr-Latn%22,%22team%22:%22sr-Latn%22,%22market%22:%22sr-Latn%22%7D&timezone=Europe%2FBelgrade&company=%7B%7D&companyUuid=4f54c6aa-82a9-475d-bf0e-dc02ded89225&filter[sportId]=18&filter[from]="
                + currentTime 
                + "&sort=categoryPosition,categoryName,tournamentPosition,tournamentName,startsAt&offerTemplate=WEB_OVERVIEW&shortProps=1";

            var client = new RestClient(url);

            var request = new RestRequest("", Method.Get);

            RestResponse response = client.Execute(request);

            JsonMatchIdsResponse matchResponse = JsonConvert.DeserializeObject<JsonMatchIdsResponse>(response.Content);

            return matchResponse;
        }

        public JsonMatchResponse GetMatchResponse(long matchId)
        {
            Thread.Sleep(Constants.SleepTimeShort);
            string url = "https://sports-sm-distribution-api.de-2.nsoftcdn.com/api/v1/events/" + matchId + "?companyUuid=4f54c6aa-82a9-475d-bf0e-dc02ded89225&id_str=" + matchId + "&language=%7B%22default%22:%22sr-Latn%22,%22events%22:%22sr-Latn%22,%22sport%22:%22sr-Latn%22,%22category%22:%22sr-Latn%22,%22tournament%22:%22sr-Latn%22,%22team%22:%22sr-Latn%22,%22market%22:%22sr-Latn%22%7D&timezone=Europe%2FBelgrade&dataFormat=%7B%22default%22:%22array%22,%22markets%22:%22array%22,%22events%22:%22array%22%7D";

            var client = new RestClient(url);

            var request = new RestRequest("", Method.Get);

            RestResponse response = client.Execute(request);

            JsonMatchResponse matchResponse = JsonConvert.DeserializeObject<JsonMatchResponse>(response.Content);

            return matchResponse;
        }
    }
}
