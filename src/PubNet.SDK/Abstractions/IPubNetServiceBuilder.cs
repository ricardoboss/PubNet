using Microsoft.Extensions.DependencyInjection;

namespace PubNet.SDK.Abstractions;

public interface IPubNetServiceBuilder
{
	IServiceCollection Services { get; }
}

public interface IPubNetApiServiceBuilder : IPubNetServiceBuilder;
