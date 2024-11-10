namespace PubNet.API.Services.Extensions;

public static class StringExtensions
{
	public static string? ToNullIfEmpty(this string? value)
	{
		return string.IsNullOrWhiteSpace(value) ? null : value;
	}
}
