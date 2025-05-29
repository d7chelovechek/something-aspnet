namespace Something.AspNet.Auth.API.Exceptions;

public class CannotRemoveCurrentSessionException()
    : Exception("Current session can not be removed.");
