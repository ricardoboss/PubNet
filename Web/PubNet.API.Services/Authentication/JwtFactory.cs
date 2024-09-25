using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PubNet.API.Abstractions.Authentication;
using PubNet.Database.Entities.Auth;
using PubNet.Web;
using PubNet.Web.Models;

namespace PubNet.API.Services.Authentication;

public class JwtFactory : IJwtFactory
{
	private readonly string audience;
	private readonly string issuer;
	private readonly JwtSecurityTokenHandler jstHandler;
	private readonly JwtHeader jwtHeader;

	public JwtFactory(IConfiguration configuration)
	{
		jstHandler = new();

		var secretKey = GetSecretKey(configuration);
		var credentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
		jwtHeader = new(credentials);

		issuer = GetIssuer(configuration);
		audience = GetAudience(configuration);
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

	public JsonWebToken Create(Token token)
	{
		var claims = new List<Claim>
		{
			new(JwtClaims.Token, token.Value),
			new(JwtClaims.Scope, string.Join(JwtClaims.ScopeSeparator, token.Scopes)),
			new(JwtClaims.Email, token.Identity.Email),
			new(JwtClaims.Username, token.Identity.Author.UserName),
		};

		var jst = new JwtSecurityToken(
			jwtHeader,
			new(
				issuer,
				audience,
				claims,
				DateTimeOffset.UtcNow.DateTime,
				token.ExpiresAtUtc.DateTime
			)
		);

		var jwtValue = jstHandler.WriteToken(jst)!;

		return JsonWebToken.From(jwtValue);
	}
}
