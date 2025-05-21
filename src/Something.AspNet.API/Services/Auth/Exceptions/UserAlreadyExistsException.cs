namespace Something.AspNet.API.Services.Auth.Exceptions;

public class UserAlreadyExistsException() 
    : Exception("User with same name already exists.");
