namespace TodoList.Application.Exceptions;

public class TaskNotFoundException : Exception
{
    public string TaskId { get; }

    public TaskNotFoundException(string taskId)
        : base($"Task with ID '{taskId}' was not found.")
    {
        TaskId = taskId;
    }
}
