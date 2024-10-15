using PubNet.Auth.Models;

namespace PubNet.Client.Abstractions;

public record TokenChangedEventArgs(JsonWebToken? Token);
