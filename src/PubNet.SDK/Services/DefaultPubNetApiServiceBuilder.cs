using Microsoft.Extensions.DependencyInjection;
using PubNet.SDK.Abstractions;

namespace PubNet.SDK.Services;

internal sealed class DefaultPubNetApiServiceBuilder(IServiceCollection services) : IPubNetApiServiceBuilder
{
	public IServiceCollection Services { get; } = services;
}
