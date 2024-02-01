using BitstampListener.Models;
using BitstampListener.Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitstampListener.Repositories
{
    public interface ITransactionRepository
    {
        public Task Create(TxModel tx);
        public Task CreateMany(List<TxModel> txs);
        public Task<TxModel> ReadById(string txId);
        public Task<TxModel> ReadByAddress(string address);
        //public Task<List<TxModel>> Read(string login, string dateFrom = "all", string dateTo = "all");
        public Task Delete(string txId);
    }
}
