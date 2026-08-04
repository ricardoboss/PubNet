namespace PubNet.API.DTO;

/// <summary>
/// An error DTO that carries the message to report when a call site does not supply one.
/// </summary>
/// <remarks>
/// Deliberately a static member: the default message is how the server picks the reported message, it is not
/// part of the response. As an instance property it was serialised into every error body and turned up in the
/// OpenAPI document and the generated clients, duplicating the message it had just produced.
/// </remarks>
public interface IHaveDefaultMessage
{
	static abstract string DefaultMessage { get; }
}
