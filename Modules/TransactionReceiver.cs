using BitstampListener.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BitstampListener
{
    public class TransactionReceiver
    {
        public async Task<List<ParsedTransactionModel>> ReceiveTransactions(List<string> addrSlice)
        {
            string content = "";
            using (var httpClient = new HttpClient())
            {
                var url = $"https://blockchain.info/multiaddr?active=" + String.Join("|", addrSlice);
                
                var result = await httpClient.GetAsync($"https://blockchain.info/multiaddr?active={String.Join('|', addrSlice)}");
                //var result = await httpClient.GetAsync($"https://blockchain.info/rawaddr/{address}");
                content = await result.Content.ReadAsStringAsync();
            }

            List<ParsedTransactionModel> transactionsList = new List<ParsedTransactionModel>();
            if (!String.IsNullOrEmpty(content))
            {
                TransactionsModel transactions = new();
                try
                {
                    transactions = JsonConvert.DeserializeObject<TransactionsModel>(content)!;
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(ex);
                    Console.ResetColor();
                    return null;
                }
                
                foreach(var tx in transactions.Txs)
                {
                    foreach(var outElem in tx.Out)
                    {
                        outElem.Date = UnixTimeStampToDateTime(tx.Time);
                    }    
                }

                var outs = transactions.Txs.SelectMany(x => x.Out).ToList();
                foreach(var address in addrSlice)
                {
                    var addrOuts = outs.Where(x => x.Addr == address).OrderBy(x => x.Date).ToList();
                    foreach(var item in addrOuts)
                    {
                        ParsedTransactionModel transactionItem = new ParsedTransactionModel
                        {
                            Address = item.Addr,
                            Value = Convert.ToDecimal(item.Value, CultureInfo.InvariantCulture) / 100000000,
                            Date = item.Date,
                            Index = item.Tx_index
                        };
                        transactionsList.Add(transactionItem);
                    }
                }
            }
            return transactionsList;
        }

        public static DateTimeOffset UnixTimeStampToDateTime(long unixTimeStamp)
        {
            DateTimeOffset dateTime = DateTimeOffset.FromUnixTimeSeconds(unixTimeStamp);
            //dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dateTime;
        }
    }
}
