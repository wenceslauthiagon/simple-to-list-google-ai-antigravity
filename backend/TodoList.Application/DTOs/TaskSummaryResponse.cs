namespace TodoList.Application.DTOs;

public record TaskSummaryResponse(
    int Total,
    int Pending,
    int InProgress,
    int Completed,
    int ProgressPercentage);
