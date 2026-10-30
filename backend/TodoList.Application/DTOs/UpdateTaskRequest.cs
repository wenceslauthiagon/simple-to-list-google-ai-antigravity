using System.ComponentModel.DataAnnotations;
using TodoList.Domain.Constants;
using TodoList.Domain.Enums;

namespace TodoList.Application.DTOs;

public record UpdateTaskRequest(
    [StringLength(TaskConstants.MaxTitleLength, MinimumLength = TaskConstants.MinTitleLength, ErrorMessage = "Title must be between 1 and 200 characters.")]
    string? Title = null,

    TaskPriority? Priority = null,

    TaskStatus? Status = null);
