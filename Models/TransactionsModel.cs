using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace BitstampListener.Models
{
    public class TransactionsModel
    {
        public string Hash160 { get; set; } = "";
        public string Address { get; set; } = "";
        public long N_tx { get; set; }
        public long N_unredeemed { get; set; }
        public long Total_Received { get; set; }
        public long Total_Sent { get; set; }
        public long Final_Balance { get; set; }
        public List<Tx> Txs { get; set; } = new List<Tx>();
    }

    public class Tx
    {
        public string Hash { get; set; } = "";
        public long Ver { get; set; }
        public long Vin_sz { get; set; }
        public long Vout_sz { get; set; }
        public long Size { get; set; }
        public long Weight { get; set; }
        public long Fee { get; set; }
        public string Relayed_by { get; set; } = "";
        public long Lock_time { get; set; }
        public long Tx_index { get; set; }
        public bool Double_spend { get; set; }
        public long Time { get; set; }
        public long Block_index { get; set; }
        public long Block_height { get; set; }
        public dynamic[] Inputs { get; set; }
        public Out[] Out { get; set; }
        public long Result { get; set; }
        public long Balance { get; set; }
    }

    public class Out
    {
        public int Type { get; set; }
        public bool Spent { get; set; }
        public long Value { get; set; }
        public dynamic Spending_outpoints { get; set; }
        public int N { get; set; }
        public long Tx_index { get; set; }
        public string Script { get; set; }
        public string Addr { get; set; }
        public DateTimeOffset Date { get; set; }
    }
}
