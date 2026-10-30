using TodoList.Application.Interfaces;
using TodoList.Domain.Entities;
using TodoList.Domain.Enums;

namespace TodoList.Tests.Fakes;

public class InMemoryTaskRepository : ITaskRepository
{
    private readonly List<TodoTask> _tasks = [];

    public Task<TodoTask?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var task = _tasks.FirstOrDefault(t => t.Id == id);
        return Task.FromResult(task);
    }

    public Task<IReadOnlyList<TodoTask>> ListAsync(
        TaskStatus? status,
        string? search,
        CancellationToken cancellationToken = default)
    {
        var query = _tasks.AsEnumerable();

        if (status.HasValue)
        {
            query = query.Where(t => t.Status == status.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var trimmedSearch = search.Trim();
            query = query.Where(t => t.Title.Contains(trimmedSearch, StringComparison.OrdinalIgnoreCase));
        }

        var result = query
            .OrderByDescending(t => t.Priority)
            .ThenByDescending(t => t.CreatedAt)
            .ToList();

        return Task.FromResult<IReadOnlyList<TodoTask>>(result);
    }

    public Task<TodoTask> AddAsync(TodoTask task, CancellationToken cancellationToken = default)
    {
        _tasks.Add(task);
        return Task.FromResult(task);
    }

    public Task<TodoTask> UpdateAsync(TodoTask task, CancellationToken cancellationToken = default)
    {
        var index = _tasks.FindIndex(t => t.Id == task.Id);
        if (index >= 0)
        {
            _tasks[index] = task;
        }
        else
        {
            _tasks.Add(task);
        }

        return Task.FromResult(task);
    }

    public Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        var removed = _tasks.RemoveAll(t => t.Id == id);
        return Task.FromResult(removed > 0);
    }

    public Task<int> ClearCompletedAsync(CancellationToken cancellationToken = default)
    {
        var count = _tasks.RemoveAll(t => t.Status == TaskStatus.Completed);
        return Task.FromResult(count);
    }

    public Task<IReadOnlyDictionary<TaskStatus, int>> GetStatusCountsAsync(CancellationToken cancellationToken = default)
    {
        var counts = new Dictionary<TaskStatus, int>
        {
            [TaskStatus.Pending] = _tasks.Count(t => t.Status == TaskStatus.Pending),
            [TaskStatus.InProgress] = _tasks.Count(t => t.Status == TaskStatus.InProgress),
            [TaskStatus.Completed] = _tasks.Count(t => t.Status == TaskStatus.Completed)
        };

        return Task.FromResult<IReadOnlyDictionary<TaskStatus, int>>(counts);
    }
}
