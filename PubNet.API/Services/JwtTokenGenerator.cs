using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using PubNet.Models;

namespace PubNet.API.Services;

public class JwtTokenGenerator
{
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
        var lifetimeInSecondsString = configuration["JWT:LifetimeInSeconds"] ?? throw new("Key 'JWT:LifetimeInSeconds' is missing in configuration");
        if (!int.TryParse(lifetimeInSecondsString, out var lifetime))
            throw new("Key 'JWT:LifetimeInSeconds' does not contain a valid integer");

        return lifetime;
    }

    private readonly List<Claim> _defaultClaims;
    private readonly int _tokenLifetimeInSeconds;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly JwtHeader _jwtHeader;
    private readonly JwtSecurityTokenHandler _jstHandler;

    public JwtTokenGenerator(IConfiguration configuration)
    {
        _defaultClaims = new();
        _jstHandler = new();

        var secretKey = GetSecretKey(configuration);
        var credentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
        _jwtHeader = new(credentials);

        _tokenLifetimeInSeconds = GetLifetimeInSeconds(configuration);
        _issuer = GetIssuer(configuration);
        _audience = GetAudience(configuration);
    }

    public JwtTokenGenerator AddDefaultClaim(Claim claim)
    {
        _defaultClaims.Add(claim);

        return this;
    }

    public string Generate(Author author, out DateTimeOffset expireTime, IEnumerable<Claim>? additionalClaims = null)
    {
        var issueTime = DateTimeOffset.Now;
        expireTime = issueTime.AddSeconds(_tokenLifetimeInSeconds);

        var claims = new List<Claim>();
        claims.AddRange(_defaultClaims);
        claims.AddRange(new Claim[]
        {
            new (ClaimTypes.Sid, author.Id.ToString()),
            new(ClaimTypes.Name, author.Name),
        });

        if (additionalClaims != null)
            claims.AddRange(additionalClaims);

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