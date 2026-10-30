namespace TodoList.Application.Exceptions;

public class ValidationException : Exception
{
    public IReadOnlyDictionary<string, string[]> Errors { get; }

    public ValidationException(string message) : base(message)
    {
        Errors = new Dictionary<string, string[]> { { "general", [message] } };
    }

    public ValidationException(string field, string message) : base(message)
    {
        Errors = new Dictionary<string, string[]> { { field, [message] } };
    }

    public ValidationException(IReadOnlyDictionary<string, string[]> errors)
        : base("One or more validation errors occurred.")
    {
        Errors = errors;
    }
}
