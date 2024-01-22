namespace PubNet.API.Abstractions.Commands;

public interface IAsyncCommandHandler<in TCommand, TResult> where TCommand : ICommand
{
	Task<TResult> HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}
