namespace PubNet.API.Abstractions.Authentication;

public class EmailAlreadyExistsException(string email) : Exception($"E-mail address {email} already exists");
