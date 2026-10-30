using TodoList.Application.DTOs;
using TodoList.Domain.Enums;

namespace TodoList.Application.Interfaces;

public interface ITaskService
{
    Task<IReadOnlyList<TaskResponse>> ListTasksAsync(TaskStatus? status, string? search, CancellationToken cancellationToken = default);
    Task<TaskResponse> GetTaskByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<TaskResponse> CreateTaskAsync(CreateTaskRequest request, CancellationToken cancellationToken = default);
    Task<TaskResponse> UpdateTaskAsync(string id, UpdateTaskRequest request, CancellationToken cancellationToken = default);
    Task<TaskResponse> AdvanceTaskStatusAsync(string id, CancellationToken cancellationToken = default);
    Task DeleteTaskAsync(string id, CancellationToken cancellationToken = default);
    Task<int> ClearCompletedTasksAsync(CancellationToken cancellationToken = default);
    Task<TaskSummaryResponse> GetSummaryAsync(CancellationToken cancellationToken = default);
}
