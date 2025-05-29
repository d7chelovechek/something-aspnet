namespace Something.AspNet.MessageBroker.Models;

public enum SessionUpdatedEventType
{
    Created = 0,
    Refreshed = 1,
    Expired = 2,
    Finished = 3
}