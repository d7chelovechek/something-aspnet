namespace Something.AspNet.API.Exceptions;

public class UserAlreadyExistsException() 
    : Exception("User with same name already exists.");
