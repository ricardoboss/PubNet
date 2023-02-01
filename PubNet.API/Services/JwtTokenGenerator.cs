using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using PubNet.Database.Models;

namespace PubNet.API.Services;

public class JwtTokenGenerator
{
	private readonly string _audience;
	private readonly string _issuer;
	private readonly JwtSecurityTokenHandler _jstHandler;
	private readonly JwtHeader _jwtHeader;
	private readonly ILogger<JwtTokenGenerator> _logger;

	private readonly int _tokenLifetimeInSeconds;

	public JwtTokenGenerator(IConfiguration configuration, ILogger<JwtTokenGenerator> logger)
	{
		_logger = logger;

		_jstHandler = new();

		var secretKey = GetSecretKey(configuration);
		var credentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
		_jwtHeader = new(credentials);

		_tokenLifetimeInSeconds = GetLifetimeInSeconds(configuration);
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

	public static int GetLifetimeInSeconds(IConfiguration configuration)
	{
		var lifetimeInSecondsString = configuration["JWT:LifetimeInSeconds"] ??
			throw new("Key 'JWT:LifetimeInSeconds' is missing in configuration");
		if (!int.TryParse(lifetimeInSecondsString, out var lifetime))
			throw new("Key 'JWT:LifetimeInSeconds' does not contain a valid integer");

		return lifetime;
	}

	public string Generate(Author author, out DateTimeOffset expireTime, IEnumerable<Claim>? additionalClaims = null)
	{
		var issueTime = DateTimeOffset.Now;
		expireTime = issueTime.AddSeconds(_tokenLifetimeInSeconds);

		var claims = new List<Claim>();
		claims.AddRange(new Claim[]
		{
			new("id", author.Id.ToString()),
			new("name", author.Name),
			new("username", author.UserName ?? string.Empty),
		});

		if (additionalClaims != null)
			claims.AddRange(additionalClaims);

		_logger.LogDebug("Generated new token for {Author} with claims: {Claims}", author.UserName, claims);

		return _jstHandler.WriteToken(
			new JwtSecurityToken(
				_jwtHeader,
				new(
					_issuer,
					_audience,
					claims,
					issueTime.DateTime,
					expireTime.DateTime
				)
			)
		);
	}
}
