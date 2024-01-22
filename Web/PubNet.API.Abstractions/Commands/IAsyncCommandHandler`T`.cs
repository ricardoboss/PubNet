namespace PubNet.API.Abstractions.Commands;

public interface IAsyncCommandHandler<in T> where T : ICommand
{
	Task HandleAsync(T command, CancellationToken cancellationToken = default);
}
