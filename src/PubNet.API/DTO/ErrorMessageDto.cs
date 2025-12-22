using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace PubNet.API.DTO;

[PublicAPI]
public abstract class ErrorMessageDto
{
	// MIND: can't use required keyword here because ErrorMessageDto needs new(), hence no required properties
	[Required]
	public CodeMessageDto Error { get; set; } = null!;
}
