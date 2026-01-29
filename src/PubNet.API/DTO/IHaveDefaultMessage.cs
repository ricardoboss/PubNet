using System.Runtime.Serialization;

namespace PubNet.API.DTO;

public interface IHaveDefaultMessage
{
	[IgnoreDataMember]
	string DefaultMessage { get; }
}
