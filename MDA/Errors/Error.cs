namespace MDA.Errors;

public class Error
{
    public readonly string Id;
    public readonly string Message;
    public readonly ErrorCategory Category;
    public readonly ErrorLevel Level;
    public readonly string? Description;
    public readonly string[]? Suggestions;
    
    public Error(string id, string message, ErrorCategory category, ErrorLevel level, string? description = null, string[]? suggestions = null)
    {
        Id = id;
        Message = message;
        Category = category;
        Level = level;
        Description = description;
        Suggestions = suggestions;
    }
}