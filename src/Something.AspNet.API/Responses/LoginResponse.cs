namespace Something.AspNet.API.Responses;

public record LoginResponse(TokenResponse AccessToken, TokenResponse RefreshToken);