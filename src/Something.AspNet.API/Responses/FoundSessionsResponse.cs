namespace Something.AspNet.API.Responses;

public record FoundSessionsResponse(IEnumerable<FoundSession> Sessions);