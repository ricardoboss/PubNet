using PubNet.Web.Models;

namespace PubNet.Client.Abstractions;

public record TokenChangedEventArgs(JsonWebToken? Token);
