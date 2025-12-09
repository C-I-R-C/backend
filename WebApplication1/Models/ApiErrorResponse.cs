public class ApiErrorResponse
{

    public string Type { get; set; }
    
    
    public string Message { get; set; }
    
    
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    
    public ApiErrorResponse(string type, string message)
    {
        Type = type;
        Message = message;
    }

}