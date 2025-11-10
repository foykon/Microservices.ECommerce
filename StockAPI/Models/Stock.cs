using MongoDB.Bson.Serialization.Attributes;

namespace StockAPI.Models
{
    public class Stock
    {
        [BsonId]
        [BsonGuidRepresentation(MongoDB.Bson.GuidRepresentation.Standard)]
        [BsonElement(Order =0)]
        public Guid Id { get; set; }

        [BsonGuidRepresentation(MongoDB.Bson.GuidRepresentation.Standard)]
        [BsonElement(Order = 1)]
        public Guid ProductId { get; set; }

        [BsonElement(Order = 2)]
        public int Count { get; set; }
    }
}
