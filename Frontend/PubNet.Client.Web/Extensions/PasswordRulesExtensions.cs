using System.Diagnostics;
using System.Text;
using PasswordRulesSharp.Models;
using PasswordRulesSharp.Rules;

namespace PubNet.Client.Web.Extensions;

public static class PasswordRulesExtensions
{
	public static string ToPasswordrulesAttribute(this IRule rule)
	{
		var sb = new StringBuilder();

		if (rule.Required is { Count: > 0 } r)
		{
			sb.Append("required: ");

			var requiredBuilder = new StringBuilder();
			foreach (var characterClass in r)
			{
				if (characterClass == CharacterClass.Unicode)
				{
					requiredBuilder.Append("unicode");
				}
				else if (characterClass == CharacterClass.AsciiPrintable)
				{
					requiredBuilder.Append("ascii-printable");
				}
				else if (characterClass == CharacterClass.Digit)
				{
					requiredBuilder.Append("digit");
				}
				else if (characterClass == CharacterClass.Lower)
				{
					requiredBuilder.Append("lower");
				}
				else if (characterClass == CharacterClass.Upper)
				{
					requiredBuilder.Append("upper");
				}
				else // assume special characters
				{
					requiredBuilder.Append("special");
				}

				requiredBuilder.Append(", ");
			}

			var required = requiredBuilder.ToString();

			sb.Append(required[..^2]);
			sb.Append(';');
		}

		if (rule.MaxConsecutive is { } mc)
		{
			Debug.Assert(mc >= 0, "Max consecutive must be greater than or equal to 0");

			sb.Append($"max-consecutive: {mc};");
		}


		return sb.ToString();
	}
}
