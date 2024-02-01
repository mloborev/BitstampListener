using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitstampListener.Models;
using BitstampListener.Repositories.Models;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using NBitcoin;
using static NBitcoin.Protocol.Behaviors.ChainBehavior;

namespace BitstampListener.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        public DBConnectionModel connectionInfo { get; set; }

        private MongoClient _mongoClient;
        private IMongoDatabase _database;
        private IMongoCollection<TxModel> _txTable;

        public TransactionRepository()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json").Build();
            var section = config.GetSection("DBSettings");
            connectionInfo = section.Get<DBConnectionModel>()!;

            var connectionString = connectionInfo.ConnectionString;
            var dbName = connectionInfo.DBName;
            var collectionName = connectionInfo.CollectionName;

            _mongoClient = new MongoClient(connectionString);
            _database = _mongoClient.GetDatabase(dbName);
            _txTable = _database.GetCollection<TxModel>(collectionName);
        }

        public async Task Create(TxModel tx)
        {
            await _txTable.ReplaceOneAsync(x => x.Id == tx.Id, tx, new ReplaceOptions() { IsUpsert = true });
        }

        public async Task CreateMany(List<TxModel> txs)
        {
            List<WriteModel<TxModel>> listWriteModels = new List<WriteModel<TxModel>>();

            foreach(var tx in txs)
            {
                listWriteModels.Add(new ReplaceOneModel<TxModel>(Builders<TxModel>.Filter.Eq(x => x.Address, tx.Address), tx) { IsUpsert = true});
            }

            /*foreach(var tx in txs)
            {
                await _txTable.ReplaceOneAsync(x => x.Address == tx.Address, tx, new ReplaceOptions() { IsUpsert = true });
                await Task.Delay(250);
            }*/
            if(listWriteModels.Any())
            {
                await _txTable.BulkWriteAsync(listWriteModels);
            }
        }

        public async Task<List<TxModel>> GetLastDateForMany(List<string> addresses)
        {
            var result = await _txTable.FindAsync(x => addresses.Contains(x.Address));
            return result.ToList();
        }

        public async Task<TxModel> ReadById(string txId)
        {
            return await _txTable.Find(x => x.Id == txId).FirstOrDefaultAsync();
        }

        public async Task<List<TxModel>> Read(string login, string dateFrom = "all", string dateTo = "all")
        {
            throw new NotImplementedException();
        }

        public async Task Delete(string txId)
        {
            throw new NotImplementedException();
        }

        public async Task<TxModel> ReadByAddress(string address)
        {
            return await _txTable.Find(x => x.Address == address).FirstOrDefaultAsync();
        }
    }
}
