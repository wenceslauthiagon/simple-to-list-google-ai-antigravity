using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TodoList.Infrastructure.Persistence.Models;

public class TaskDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public string Priority { get; set; } = string.Empty;

    public int PriorityLevel { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
