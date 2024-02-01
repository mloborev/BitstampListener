using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitstampListener.Repositories.Models
{
    public class TxModel
    {
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonIgnoreIfDefault]
        public string Id { get; set; }/* = ObjectId.GenerateNewId().ToString();*/
        public long Index { get; set; }
        public DateTimeOffset Date { get; set; }
        public decimal Value { get; set; }
        public string Address { get; set; } = "";
    }
}
