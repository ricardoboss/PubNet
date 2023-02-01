using System.ComponentModel.DataAnnotations;

namespace PubNet.API.DTO;

public class RegisterRequest
{
	[Required] public string? Username { get; set; }

	[Required] public string? Email { get; set; }

	[Required] public string? Password { get; set; }

	[Required] public string? Name { get; set; }

	public string? Website { get; set; }

	public void Deconstruct(out string? username, out string? email, out string? password, out string? name, out string? website)
	{
		username = Username;
		email = Email;
		password = Password;
		name = Name;
		website = Website;
	}
}
