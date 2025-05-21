namespace Something.AspNet.API.Responses;

public record TokenResponse(string Token, DateTime ExpiredAt);
