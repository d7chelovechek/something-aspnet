namespace Something.AspNet.API.Exceptions;

public class CannotRemoveCurrentSessionException()
    : Exception("Current session can not be removed.");
