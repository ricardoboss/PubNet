using System.Text;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using PubNet.API.Contexts;
using PubNet.API.Models;

namespace PubNet.API.Services;

public class BearerTokenManager
{
    public enum VerifyResult
    {
        InvalidFormat,
        UnknownAuthor,
        UnknownToken,
        ExpiredToken,
        Ok,
    }

    private readonly PubNetContext _db;
    private readonly IDataProtectionProvider _dataProtectionProvider;

    public BearerTokenManager(PubNetContext db, IDataProtectionProvider dataProtectionProvider)
    {
        _db = db;
        _dataProtectionProvider = dataProtectionProvider;
    }

    public string Generate(AuthorToken authorToken)
    {
        var builder = new MemoryStream();
        builder.Write(Encoding.UTF8.GetBytes(authorToken.Owner.Email));
        builder.Write(new byte[]{ 0x0 });
        builder.Write(authorToken.Value);

        var raw = builder.ToArray();
        var encrypted = GetProtector().Protect(raw);

        return Convert.ToBase64String(encrypted);
    }

    public VerifyResult Verify(string token, out AuthorToken? authorToken)
    {
        authorToken = null;

        byte[] decrypted;
        try
        {
            var decoded = Convert.FromBase64String(token);
            decrypted = GetProtector().Unprotect(decoded);
        }
        catch (Exception)
        {
            return VerifyResult.InvalidFormat;
        }

        if (!decrypted.Contains((byte)0x0))
        {
            return VerifyResult.InvalidFormat;
        }

        int nullIndex;
        for (nullIndex = 0; nullIndex < decrypted.Length && decrypted[nullIndex] != 0x0; nullIndex++)
        {
        }

        var emailBuffer = new byte[nullIndex];
        Array.Copy(decrypted, emailBuffer, emailBuffer.Length);
        var email = Encoding.UTF8.GetString(emailBuffer);
        var author = _db.Authors.Where(a => a.Email == email).Include(a => a.Tokens).FirstOrDefault();
        if (author is null) return VerifyResult.UnknownAuthor;

        var tokenValue = new byte[decrypted.Length - emailBuffer.Length - 1];
        Array.Copy(decrypted, nullIndex + 1, tokenValue, 0, tokenValue.Length);
        authorToken = author.Tokens.FirstOrDefault(t => t.Value.SequenceEqual(tokenValue));
        if (authorToken is null) return VerifyResult.UnknownToken;

        return !authorToken.IsValid() ? VerifyResult.ExpiredToken : VerifyResult.Ok;
    }

    private IDataProtector GetProtector()
    {
        return _dataProtectionProvider.CreateProtector(nameof(BearerTokenManager));
    }
}