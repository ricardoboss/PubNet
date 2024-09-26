namespace PubNet.API.Abstractions.Authentication;

public class UserNameAlreadyExistsException(string userName) : Exception($"User name '{userName}' already exists.");
