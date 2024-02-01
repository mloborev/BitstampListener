using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitstampListener.Models
{
    public class BitstampBTCExchangeRateModel
    {
        public List<Data>? Data { get; set; }
        public object? Pagination { get; set; }
    }

    public class Data
    {
        public string Close { get; set; } = "";
        public string High { get; set; } = "";
        public string Low { get; set; } = "";
        public string Open { get; set; } = "";
        public string Time { get; set; } = "";
        public string TimeStamp { get; set; } = "";
        public string Volume { get; set; } = "";
    }
}
