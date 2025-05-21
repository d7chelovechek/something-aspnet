namespace Something.AspNet.API.Services.Exceptions
{
    public class UserAlreadyExistsException() 
        : Exception("User with same name already exists.");
}
