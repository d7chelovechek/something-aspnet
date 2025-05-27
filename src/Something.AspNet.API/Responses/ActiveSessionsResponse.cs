namespace Something.AspNet.API.Responses;

public record ActiveSessionsResponse(IEnumerable<Guid> SessionIds);