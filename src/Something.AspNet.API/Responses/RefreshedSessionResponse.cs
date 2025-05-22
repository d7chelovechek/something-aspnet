namespace Something.AspNet.API.Responses;

public record RefreshedSessionResponse(string AccessToken, string RefreshToken, long ExpiresAt);
