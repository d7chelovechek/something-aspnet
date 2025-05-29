namespace Something.AspNet.Auth.API.Responses;

public record ActiveSessionsResponse(IEnumerable<Guid> SessionIds);