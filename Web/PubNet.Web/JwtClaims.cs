namespace PubNet.Web;

public static class JwtClaims
{
	public const char ScopeSeparator = ' ';

	public const string IdentityId = "sub";
	public const string TokenValue = "jti";
	public const string Scopes = "scope";
	public const string AuthorUsername = "preferred_username";
}
