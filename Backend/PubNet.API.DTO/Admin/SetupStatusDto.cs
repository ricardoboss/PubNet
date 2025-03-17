using System.ComponentModel.DataAnnotations;

namespace PubNet.API.DTO.Admin;

public class SetupStatusDto
{
	[Required]
	public required bool RegistrationsOpen { get; set; }

	[Required]
	public required bool SetupComplete { get; set; }
}
