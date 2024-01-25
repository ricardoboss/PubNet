using System.Diagnostics;
using Vogen;

namespace PubNet.Web.Abstractions.Models;

[ValueObject<string>(conversions: Conversions.SystemTextJson)]
[Instance("Any", "*")]
public readonly partial struct Scope
{
	private const char SeparatorChar = ':';
	private const char AnyChar = '*';
	private const string DoubleSeparator = "::";

	public static Scope operator +(Scope left, Scope right)
	{
		Debug.Assert(left.Value != null, nameof(left.Value) + " != null");
		Debug.Assert(right.Value != null, nameof(right.Value) + " != null");

		return From(left.Value + SeparatorChar + right.Value);
	}

	public static Scope operator +(Scope left, string right)
	{
		Debug.Assert(left.Value != null, nameof(left.Value) + " != null");

		return From(left.Value + SeparatorChar + right);
	}

	public bool IsParentOf(Scope scope)
	{
		Debug.Assert(Value != null, nameof(Value) + " != null");
		Debug.Assert(scope.Value != null, nameof(scope.Value) + " != null");

		var segments = Value.Split(SeparatorChar);
		var otherSegments = scope.Value.Split(SeparatorChar);

		var minSegments = Math.Min(segments.Length, otherSegments.Length);
		for (var i = 0; i < minSegments; i++)
			if (!string.Equals(segments[i], otherSegments[i], StringComparison.Ordinal))
				return segments[i] == Any || otherSegments[i] == Any;

		return false;
	}

	public bool EqualsOrIsParentOf(Scope scope)
	{
		Debug.Assert(Value != null, nameof(Value) + " != null");
		Debug.Assert(scope.Value != null, nameof(scope.Value) + " != null");

		return string.Equals(scope.Value, Value, StringComparison.Ordinal) || IsParentOf(scope);
	}

	private static Validation Validate(string input)
	{
		if (input.Length == 0)
			return Validation.Invalid("Scope cannot be empty.");

		if (input.Trim() != input)
			return Validation.Invalid("Scope cannot have leading or trailing whitespace.");

		var firstInvalid = input.FirstOrDefault(c => !char.IsAsciiLetterLower(c) && c != SeparatorChar && c != AnyChar);
		if (firstInvalid != default)
			return Validation.Invalid("Scope can only contain lowercase letters and the separator character, but found: " + firstInvalid + " (" + (int)firstInvalid + ")");

		if (input[0] == SeparatorChar || input[^1] == SeparatorChar)
			return Validation.Invalid("Scope cannot start or end with the separator character.");

		if (input.Contains(DoubleSeparator, StringComparison.Ordinal))
			return Validation.Invalid("Scope cannot contain multiple consecutive separator characters.");

		if (input.IndexOf(AnyChar) != input.LastIndexOf(AnyChar))
			return Validation.Invalid("Scope cannot contain multiple wildcard characters.");

		if (input.Contains(AnyChar) && input[^1] != AnyChar)
			return Validation.Invalid("Scope can only contain the wildcard character at the end.");

		return Validation.Ok;
	}
}
