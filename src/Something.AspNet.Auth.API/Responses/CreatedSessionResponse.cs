namespace Something.AspNet.Auth.API.Responses;

public record CreatedSessionResponse(string AccessToken, string RefreshToken, long ExpiresAt);