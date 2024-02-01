using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitstampListener.Repositories.Models
{
    public class DBConnectionModel
    {
        public string ConnectionString { get; set; } = "";
        public string DBName { get; set; } = "";
        public string CollectionName { get; set; } = "";
    }
}
