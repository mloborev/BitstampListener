using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitstampListener.Models
{
    public class ParsedTransactionModel
    {
        public long Index { get; set; }
        public DateTimeOffset Date { get; set; }
        public decimal Value { get; set; }
        public string Address { get; set; } = "";
    }
}
