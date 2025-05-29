namespace Something.AspNet.Auth.API.Responses;

public record ErrorsResponse(IEnumerable<string> Errors);