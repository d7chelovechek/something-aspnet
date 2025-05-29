namespace Something.AspNet.Auth.API.Responses;

public record RefreshedSessionResponse(string AccessToken, string RefreshToken, long ExpiresAt);
