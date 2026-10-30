using Bogus;
using TodoList.Application.DTOs;
using TodoList.Domain.Entities;
using TodoList.Domain.Enums;

namespace TodoList.Tests.Fakes;

public static class TaskFaker
{
    private static readonly Faker Faker = new();

    public static CreateTaskRequest CreateRequest(
        string? title = null,
        TaskPriority? priority = null)
    {
        return new CreateTaskRequest(
            title ?? Faker.Commerce.ProductName(),
            priority ?? Faker.PickRandom<TaskPriority>());
    }

    public static TodoTask CreateTask(
        string? title = null,
        TaskPriority? priority = null,
        TaskStatus? status = null,
        DateTimeOffset? createdAt = null)
    {
        var task = TodoTask.Create(
            title ?? Faker.Commerce.ProductName(),
            priority ?? Faker.PickRandom<TaskPriority>(),
            createdAt ?? DateTimeOffset.UtcNow);

        if (status.HasValue && status.Value != TaskStatus.Pending)
        {
            task.ChangeStatus(status.Value, (createdAt ?? DateTimeOffset.UtcNow).AddMinutes(5));
        }

        return task;
    }
}
