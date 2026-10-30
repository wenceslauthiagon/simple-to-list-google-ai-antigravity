using TodoList.Domain.Entities;
using TodoList.Domain.Enums;

namespace TodoList.Application.Interfaces;

public interface ITaskRepository
{
    Task<TodoTask?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TodoTask>> ListAsync(TaskStatus? status, string? search, CancellationToken cancellationToken = default);
    Task<TodoTask> AddAsync(TodoTask task, CancellationToken cancellationToken = default);
    Task<TodoTask> UpdateAsync(TodoTask task, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default);
    Task<int> ClearCompletedAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyDictionary<TaskStatus, int>> GetStatusCountsAsync(CancellationToken cancellationToken = default);
}
