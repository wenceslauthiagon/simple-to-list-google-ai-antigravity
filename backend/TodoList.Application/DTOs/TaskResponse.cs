using TodoList.Domain.Entities;
using TodoList.Domain.Enums;

namespace TodoList.Application.DTOs;

public record TaskResponse(
    string Id,
    string Title,
    TaskStatus Status,
    TaskPriority Priority,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt)
{
    public static TaskResponse FromDomain(TodoTask task) =>
        new(task.Id, task.Title, task.Status, task.Priority, task.CreatedAt, task.UpdatedAt);
}
