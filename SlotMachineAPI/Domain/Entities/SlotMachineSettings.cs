using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SlotMachineAPI.Domain.Entities
{
    public class SlotMachineSettings
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = null!;

        [BsonElement("rows")]
        public int Rows { get; set; }

        [BsonElement("cols")]
        public int Cols { get; set; }
    }
}
