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

    public async Task<AuthorToken> Dispense(string name, Author owner, TimeSpan lifetime, int length = 128, bool save = true)
    {
        using var rng = RandomNumberGenerator.Create();
        var buffer = new byte[length];
        rng.GetNonZeroBytes(buffer);

        var token = new AuthorToken
        {
            OwnerId = owner.Id,
            Name = name,
            Value = Convert.ToBase64String(buffer),
            ExpiresAtUtc = DateTimeOffset.UtcNow.Add(lifetime)
        };

        owner.Tokens.Add(token);

        if (save)
        {
            await _db.SaveChangesAsync();
        }

        return token;
    }
}