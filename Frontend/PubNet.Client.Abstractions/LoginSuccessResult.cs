using PubNet.Auth.Models;

namespace PubNet.Client.Abstractions;

public record LoginSuccessResult(JsonWebToken Token);
