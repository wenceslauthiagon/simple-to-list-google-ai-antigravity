using System.ComponentModel.DataAnnotations;
using TodoList.Domain.Constants;
using TodoList.Domain.Enums;

namespace TodoList.Application.DTOs;

public record CreateTaskRequest(
    [Required(ErrorMessage = "Title is required.")]
    [StringLength(TaskConstants.MaxTitleLength, MinimumLength = TaskConstants.MinTitleLength, ErrorMessage = "Title must be between 1 and 200 characters.")]
    string Title,

    TaskPriority Priority = TaskPriority.Medium);
