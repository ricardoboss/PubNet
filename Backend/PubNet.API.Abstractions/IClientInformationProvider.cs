namespace PubNet.API.Abstractions;

public interface IClientInformationProvider
{
	string IpAddress { get; }

	string UserAgent { get; }

	string DeviceType { get; }

	string Browser { get; }

	string Platform { get; }
}
