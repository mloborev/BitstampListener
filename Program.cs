using BitstampListener;
using BitstampListener.Models;
using BitstampListener.Modules;
using BitstampListener.Modules.GoogleIntegrator;
using BitstampListener.Repositories;
using BitstampListener.Repositories.Models;
using Google.Apis.Auth.OAuth2;
using Slack.Webhooks;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks.Dataflow;
using System.Transactions;

namespace BitStampListener
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            //var image = ScreenshotMaker.CaptureDesktop();
            //image.Save(@"C:\Users\maxim\Downloads\test.jpeg", ImageFormat.Jpeg);

            //Мои
            //string googleClientId = "869330182156-pb8ausdsl3l29kvddb65g7s93jplacs0.apps.googleusercontent.com";
            //string googleClientSecret = "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";

            //Машины
            string googleClientId = "609451915545-qj46v51p1libsbcm8jco7eh5i0r6gc2b.apps.googleusercontent.com";
            string googleClientSecret = "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";
            string[] scopes = new[] { Google.Apis.Sheets.v4.SheetsService.Scope.Spreadsheets };

            UserCredential credential = await GoogleAuth.LoginAsync(googleClientId, googleClientSecret, scopes);
            GoogleSheetsManager manager = new GoogleSheetsManager(credential);

            //var newSheet = await manager.CreateNewAsync("TestApiDocument");
            //Console.WriteLine(newSheet.ToString());

            /*var mySpreadSheetId = "12l1v-WaDGicdppED7in-zdP1Vfu3WK0TkXql0zx3Qoc";
                        var spreadSheet = manager.GetSpreadSheet(mySpreadSheetId);

                        var testSingleCell = manager.GetSingleValue(spreadSheet.SpreadsheetId, "E16");
                        Console.WriteLine(testSingleCell.Values.First().First());

                        var aRange = "A1:A10";
                        var bRange = "B1:B10";
                        var cRange = "C1:C10";

                        string[] valueRanges = new[] { aRange, bRange, cRange };
                        var multipleResponse = manager.GetMultipleValues(mySpreadSheetId, valueRanges);

                        var aResponse = multipleResponse.ValueRanges.ElementAt(0);
                        var bResponse = multipleResponse.ValueRanges.ElementAt(1);
                        var cResponse = multipleResponse.ValueRanges.ElementAt(2);

                        Console.WriteLine("\n\nResult:");
                        for(int i = 0; i < aResponse.Values.Count; i++)
                        {
                            string a, b, c;
                            try
                            {
                                a = aResponse.Values[i].First().ToString();
                            }
                            catch
                            {
                                a = "null";
                            }
                            try
                            {
                                b = bResponse.Values[i].First().ToString();
                            }
                            catch
                            {
                                b = "null";
                            }
                            try
                            {
                                c = cResponse.Values[i].First().ToString();
                            }
                            catch
                            {
                                c = "null";
                            }

                            Console.WriteLine($"A{i}: {a}\tB{i}: {b}\tC{i}: {c}");
                        }*/

            //Получение списка всех биткоин адресов
            //Мои
            //var cryptoAddressesSheetId = "1KMdf_L4UeWNOwMM54rpnhnpapzgelvttCX7R09n8FN0";

            //Машины
            var cryptoAddressesSheetId = "1QnkVxuU7L0GBrbiuQSUdZbcA8OoYdfIJFqyW9KBinbw";
            var spreadSheet = manager.GetSpreadSheet(cryptoAddressesSheetId);

            var addressesRange = "адреса btc!A2:A";
            var brandsRange = "адреса btc!B2:B";

            string[] valueRanges = new[] { addressesRange, brandsRange };
            var addressesResponse = manager.GetMultipleValues(cryptoAddressesSheetId, valueRanges).ValueRanges.ElementAt(0).Values;
            var brandsResponse = manager.GetMultipleValues(cryptoAddressesSheetId, valueRanges).ValueRanges.ElementAt(1).Values;

            List<string> addresses = new List<string>();
            foreach (var address in addressesResponse)
            {
                try
                {
                    if (address[0].ToString().Trim().Contains(' '))
                    {
                        continue;
                    }
                    addresses.Add(address[0].ToString());
                }
                catch
                {
                    continue;
                }
            }

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(DateTime.Now.ToShortTimeString() + " Список адресов пошёл сначала --------------------------------------------");
                Console.ResetColor();

                List<string> addrSlice = new List<string>(10);
                for (int i = 0; i <= addresses.Count; i += 10)
                {
                    addrSlice = addresses.Skip(i).Take(10).ToList();
                    Console.WriteLine($"{DateTime.Now.ToShortTimeString()} Проверяются адреса:\n{String.Join(",\n", addrSlice)}\n-------------------------------------");
                    TransactionReceiver txReceiver = new TransactionReceiver();

                    var transactions = await txReceiver.ReceiveTransactions(addrSlice);

                    List<TxModel> txs = ParsedTxToTxModel(transactions);

                    if(txs == null)
                    {
                        continue;
                    }

                    var txRep = new TransactionRepository();
                    var txsFromDB = await txRep.GetLastDateForMany(addrSlice);

                    List<TxModel> listTxsToCompare = new List<TxModel>();

                    List<TxModel> lastDateTxsFromAPI = new List<TxModel>();
                    foreach(var address in addresses)
                    {
                        List<TxModel> listTxsOfAddress = txs.FindAll(x => x.Address == address);
                        if(!listTxsOfAddress.Any())
                        {
                            continue;
                        }
                        listTxsOfAddress.Sort((x, y) => DateTimeOffset.Compare(x.Date, y.Date));
                        lastDateTxsFromAPI.Add(listTxsOfAddress.LastOrDefault()!);
                    }

                    List<TxModel> newTransactions = new();
                    foreach (var tx in lastDateTxsFromAPI)
                    {
                        //bool isDateNear = false;

                        var transactionFromDB = txsFromDB.FirstOrDefault(x => x.Address == tx.Address);
                        /*var txDateWithoutMinutes = tx.Date.AddMinutes(-tx.Date.Minute).AddSeconds(-tx.Date.Second);
                        var txFromDBDateWithoutMinutes = transactionFromDB.Date.AddMinutes(-transactionFromDB.Date.Minute).AddSeconds(-transactionFromDB.Date.Second);
                        if(DateTimeOffset.Compare(txDateWithoutMinutes, txFromDBDateWithoutMinutes) == 0)
                        {
                            isDateNear = true;
                        }*/

                        if (transactionFromDB == null || transactionFromDB.Index != tx.Index)
                        {
                            newTransactions.Add(tx);
                        }

                        /*if (transactionFromDB == null || (DateTimeOffset.Compare(transactionFromDB.Date, tx.Date) < 0 && !(*//*isDateNear &&*//* tx.Value == transactionFromDB.Value && transactionFromDB.Index == tx.Index)))
                        {
                            newTransactions.Add(tx);
                        }*/
                    }

                    await txRep.CreateMany(newTransactions);

                    var slackIntegrator = new SlackIntegrator();
                    var slackClient = new SlackClient("https://hooks.slack.com/services/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");
                    foreach (var item in newTransactions)
                    {
                        decimal usdPrice = 0;
                        decimal eurPrice = 0;

                        var bitstampIntegrator = new BitstampIntegrator();
                        usdPrice = await bitstampIntegrator.GetBtcExchangeRate(item.Date, "USD");
                        eurPrice = await bitstampIntegrator.GetBtcExchangeRate(item.Date, "EUR");

                        string text = $"New transaction alert.\nType: Confirmed\nAddress: {item.Address}\nValue: {item.Value}\nTransaction index: {item.Index}\nTime: {item.Date.AddHours(3):dd.MM.yyyy HH:mm:ss} +03:00\n\nPrice for one bitcoin:\nUSD - {(usdPrice == 0 ? "ERROR" : usdPrice)}\nEUR - {(eurPrice == 0 ? "ERROR" : eurPrice)}";
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine(text);
                        Console.ResetColor();
                        slackIntegrator.SendMessage(slackClient, text);

                        var txsSheetId = "16DckAwmOVVsC0MH8uniYQrRlW12bkwbHPAiiAO8KW6M";
                        var txsRange = "BTC!A:N";
                        string[] txValueRanges = new[] { txsRange };
                        var oblist = new List<object>() { "Received", item.Date.ToString("dd"), item.Date.ToString("MMMM"), item.Date.ToString("yyyy"), item.Date.ToString("HH:mm"), "confirmed 1", item.Value, "", "", "", usdPrice.ToString(), "usdScreen", eurPrice.ToString(), "eurScreen" };
                        var updateResponse = manager.AddRow(txsSheetId, txsRange, oblist);
                    }
                    await Task.Delay(240000);
                }
            }
        }

        public static List<TxModel> ParsedTxToTxModel(List<ParsedTransactionModel> parsedTxs)
        {
            try
            {
                List<TxModel> txModels = new List<TxModel>();
                foreach (var parsedTx in parsedTxs)
                {
                    TxModel txModelItem = new TxModel
                    {
                        Index = parsedTx.Index,
                        Address = parsedTx.Address,
                        Date = parsedTx.Date,
                        Value = parsedTx.Value
                    };
                    txModels.Add(txModelItem);
                }
                return txModels;
            }
            catch
            {
                return null;
            }
        }
    }
}