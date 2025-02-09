using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace SlotMachineAPI.Domain
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("Username")]
        public string Username { get; set; }

        [BsonElement("Email")]
        public string Email { get; set; }

        [BsonElement("PasswordHash")]
        public string PasswordHash { get; set; }

        [BsonElement("Role")]
        public string Role { get; set; } = "User"; // Default role

        [BsonElement("RefreshToken")]
        public string? RefreshToken { get; set; }

        [BsonElement("RefreshTokenExpiryTime")]
        public DateTime? RefreshTokenExpiryTime { get; set; }
    }
}
