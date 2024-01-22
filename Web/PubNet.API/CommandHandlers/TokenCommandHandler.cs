using PubNet.API.Abstractions.Commands;
using PubNet.API.Abstractions.Commands.Authentication;

namespace PubNet.API.CommandHandlers;

public class TokenAsyncCommandHandler : IAsyncCommandHandler<CreateTokenCommand, TokenCreatedResult>
{
	public Task<TokenCreatedResult> HandleAsync(CreateTokenCommand command, CancellationToken cancellationToken = default)
	{
	}
}
