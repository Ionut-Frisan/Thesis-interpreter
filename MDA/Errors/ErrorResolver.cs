namespace MDA.Errors;

public static class ErrorResolver
{
    public static string Resolve(string id, Dictionary<string, string>? placeholders = null)
    {
        Error? error = ErrorRegistry.GetError(id);
        if (error == null)
        {
            // If the error is not found, return the id as the message
            return id;
        }
        
        string message = error.Message;
        
        if (placeholders != null)
        {
            // Replace placeholders in the message with the provided values
            foreach (KeyValuePair<string, string> placeholder in placeholders)
            {
                message = message.Replace($":{placeholder.Key}:", placeholder.Value);
            }
        }
        
        return $"[{error.Id}] - {message}";
    }
}