using BitstampListener.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitstampListener.Modules
{
    public class BitstampIntegrator
    {
        public async Task<decimal> GetBtcExchangeRate(DateTimeOffset time, string currency)
        {
            using (var httpClient = new HttpClient())
            {
                var url = @$"https://www.bitstamp.net/api-internal/tradeview/price-history/BTC/{currency}/?step=60&start_datetime={time:yyyy-MM-dd}T{time:HH:mm:ss}.000Z&end_datetime={time:yyyy-MM-dd}T{time.AddMinutes(1):HH:mm:ss}.000Z";

                var result = await httpClient.GetAsync(url);
                var content = await result.Content.ReadAsStringAsync();

                var deserializedContent = JsonConvert.DeserializeObject<BitstampBTCExchangeRateModel>(content);
                if(deserializedContent != null)
                {
                    var price = deserializedContent.Data!.Last().Close;
                    return decimal.Parse(price, CultureInfo.InvariantCulture);
                }
                return 0;
            }
        }
    }
}
