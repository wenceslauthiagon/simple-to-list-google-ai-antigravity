namespace TodoList.Application.Exceptions;

public class InvalidTaskIdException : Exception
{
    public string TaskId { get; }

    public InvalidTaskIdException(string taskId, string message = "Task ID is invalid.")
        : base(message)
    {
        TaskId = taskId;
    }
}
