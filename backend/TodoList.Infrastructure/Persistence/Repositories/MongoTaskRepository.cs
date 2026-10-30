using System.Text.RegularExpressions;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using TodoList.Application.Interfaces;
using TodoList.Domain.Entities;
using TodoList.Domain.Enums;
using TodoList.Infrastructure.Configuration;
using TodoList.Infrastructure.Persistence.Mappers;
using TodoList.Infrastructure.Persistence.Models;

namespace TodoList.Infrastructure.Persistence.Repositories;

public class MongoTaskRepository : ITaskRepository
{
    private readonly IMongoCollection<TaskDocument> _collection;

    public MongoTaskRepository(IMongoDatabase database, IOptions<MongoDbSettings> settings)
    {
        ArgumentNullException.ThrowIfNull(database);
        ArgumentNullException.ThrowIfNull(settings);

        var collectionName = string.IsNullOrWhiteSpace(settings.Value.CollectionName)
            ? "Tasks"
            : settings.Value.CollectionName;

        _collection = database.GetCollection<TaskDocument>(collectionName);
    }

    public async Task<TodoTask?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var document = await _collection
            .Find(d => d.Id == id)
            .FirstOrDefaultAsync(cancellationToken);

        return document is null ? null : TaskDocumentMapper.ToDomain(document);
    }

    public async Task<IReadOnlyList<TodoTask>> ListAsync(
        TaskStatus? status,
        string? search,
        CancellationToken cancellationToken = default)
    {
        var filterBuilder = Builders<TaskDocument>.Filter;
        var filter = filterBuilder.Empty;

        if (status.HasValue)
        {
            filter &= filterBuilder.Eq(d => d.Status, status.Value.ToString());
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var escaped = Regex.Escape(search.Trim());
            filter &= filterBuilder.Regex(d => d.Title, new BsonRegularExpression(escaped, "i"));
        }

        var sort = Builders<TaskDocument>.Sort
            .Descending(d => d.PriorityLevel)
            .Descending(d => d.CreatedAt);

        var documents = await _collection
            .Find(filter)
            .Sort(sort)
            .ToListAsync(cancellationToken);

        return documents.Select(TaskDocumentMapper.ToDomain).ToList();
    }

    public async Task<TodoTask> AddAsync(TodoTask task, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(task);

        var document = TaskDocumentMapper.ToDocument(task);
        await _collection.InsertOneAsync(document, null, cancellationToken);
        return TaskDocumentMapper.ToDomain(document);
    }

    public async Task<TodoTask> UpdateAsync(TodoTask task, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(task);

        var document = TaskDocumentMapper.ToDocument(task);
        await _collection.ReplaceOneAsync(
            d => d.Id == task.Id,
            document,
            new ReplaceOptions { IsUpsert = false },
            cancellationToken);

        return task;
    }

    public async Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        var result = await _collection.DeleteOneAsync(d => d.Id == id, cancellationToken);
        return result.DeletedCount > 0;
    }

    public async Task<int> ClearCompletedAsync(CancellationToken cancellationToken = default)
    {
        var result = await _collection.DeleteManyAsync(
            d => d.Status == TaskStatus.Completed.ToString(),
            cancellationToken);

        return (int)result.DeletedCount;
    }

    public async Task<IReadOnlyDictionary<TaskStatus, int>> GetStatusCountsAsync(CancellationToken cancellationToken = default)
    {
        var aggregate = await _collection.Aggregate()
            .Group(d => d.Status, g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        var counts = new Dictionary<TaskStatus, int>
        {
            [TaskStatus.Pending] = 0,
            [TaskStatus.InProgress] = 0,
            [TaskStatus.Completed] = 0
        };

        foreach (var item in aggregate)
        {
            if (Enum.TryParse<TaskStatus>(item.Status, ignoreCase: true, out var status))
            {
                counts[status] = (int)item.Count;
            }
        }

        return counts;
    }

    public async Task EnsureIndexesAsync(CancellationToken cancellationToken = default)
    {
        var statusIndex = Builders<TaskDocument>.IndexKeys.Ascending(d => d.Status);
        var priorityIndex = Builders<TaskDocument>.IndexKeys.Descending(d => d.PriorityLevel);
        var createdAtIndex = Builders<TaskDocument>.IndexKeys.Descending(d => d.CreatedAt);
        var compoundIndex = Builders<TaskDocument>.IndexKeys
            .Descending(d => d.PriorityLevel)
            .Descending(d => d.CreatedAt);

        await _collection.Indexes.CreateManyAsync([
            new CreateIndexModel<TaskDocument>(statusIndex, new CreateIndexOptions { Name = "idx_status" }),
            new CreateIndexModel<TaskDocument>(priorityIndex, new CreateIndexOptions { Name = "idx_priority" }),
            new CreateIndexModel<TaskDocument>(createdAtIndex, new CreateIndexOptions { Name = "idx_createdAt" }),
            new CreateIndexModel<TaskDocument>(compoundIndex, new CreateIndexOptions { Name = "idx_priority_createdAt" })
        ], cancellationToken);
    }
}
