namespace Models;

public record LoginRequest(string Email, string Password);


public record LoginResult(

    string access_token,  
    string username,
    int user_id         
    
);


public record UserProfile(int Id, string Email, DateTime CreatedAt);