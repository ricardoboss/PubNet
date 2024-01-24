using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using PubNet.API.Abstractions.Authentication;
using PubNet.Database.Entities.Auth;
using PubNet.Web.Abstractions;

namespace PubNet.API.Services;

public class JwtFactory : IJwtFactory
{
	private readonly string _audience;
	private readonly string _issuer;
	private readonly JwtSecurityTokenHandler _jstHandler;
	private readonly JwtHeader _jwtHeader;

	public JwtFactory(IConfiguration configuration)
	{
		_jstHandler = new();

		var secretKey = GetSecretKey(configuration);
		var credentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
		_jwtHeader = new(credentials);

		_issuer = GetIssuer(configuration);
		_audience = GetAudience(configuration);
	}

	public static SecurityKey GetSecretKey(IConfiguration configuration)
	{
		var keyString = configuration["JWT:SecretKey"] ?? throw new("Key 'JWT:SecretKey' is missing in configuration");
		var keyBytes = Encoding.UTF8.GetBytes(keyString);
		return new SymmetricSecurityKey(keyBytes);
	}

	public static string GetIssuer(IConfiguration configuration)
	{
		return configuration["JWT:Issuer"] ?? throw new("Key 'JWT:Issuer' is missing in configuration");
	}

	public static string GetAudience(IConfiguration configuration)
	{
		return configuration["JWT:Audience"] ?? throw new("Key 'JWT:Audience' is missing in configuration");
	}

	public string Create(Token token)
	{
		var claims = new List<Claim>
		{
			new(JwtClaims.Token, token.Value),
			new(JwtClaims.Scope, string.Join(JwtClaims.ScopeSeparator, token.Scopes)),
		};

		var jst = new JwtSecurityToken(
			_jwtHeader,
			new(
				_issuer,
				_audience,
				claims,
				DateTimeOffset.UtcNow.DateTime,
				token.ExpiresAtUtc.DateTime
			)
		);

		return _jstHandler.WriteToken(jst)!;
	}
}
