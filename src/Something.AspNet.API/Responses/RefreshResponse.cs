namespace Something.AspNet.API.Responses;

public record RefreshResponse(string AccessToken, string RefreshToken, long ExpiresAt);
