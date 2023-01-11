using System.Security.Cryptography;
using PubNet.API.Contexts;
using PubNet.API.Models;

namespace PubNet.API.Services;

public class AuthorTokenDispenser
{
    private readonly PubNetContext _db;

    public AuthorTokenDispenser(PubNetContext db)
    {
        _db = db;
    }

    public async Task<AuthorToken> Dispense(string name, Author owner, TimeSpan lifetime)
    {
        const int tokenLength = 128;

        using var rng = RandomNumberGenerator.Create();
        var buffer = new byte[tokenLength];
        rng.GetNonZeroBytes(buffer);

        var token = new AuthorToken
        {
            Owner = owner,
            Name = name,
            Value = buffer,
            ExpiresAtUtc = DateTimeOffset.UtcNow.Add(lifetime),
        };

        _db.Tokens.Add(token);

        await _db.SaveChangesAsync();

        return token;
    }
}