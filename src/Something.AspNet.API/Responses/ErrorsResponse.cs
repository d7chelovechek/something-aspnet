namespace Something.AspNet.API.Responses;

public record ErrorsResponse(IEnumerable<string> Errors);