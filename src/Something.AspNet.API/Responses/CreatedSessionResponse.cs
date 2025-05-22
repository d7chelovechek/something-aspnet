namespace Something.AspNet.API.Responses;

public record CreatedSessionResponse(string AccessToken, string RefreshToken, long ExpiresAt);