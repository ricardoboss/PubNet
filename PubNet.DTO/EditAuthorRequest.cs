namespace PubNet.API.DTO;

public class EditAuthorRequest
{
	public string? Name { get; set; }
	public string? Website { get; set; }

	public void Deconstruct(out string? name, out string? website)
	{
		name = Name;
		website = Website;
	}
}
