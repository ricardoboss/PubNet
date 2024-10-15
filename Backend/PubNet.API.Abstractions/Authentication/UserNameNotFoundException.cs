namespace PubNet.API.Abstractions.Authentication;

public class UserNameNotFoundException(string userName) : Exception("User name not found: " + userName);
