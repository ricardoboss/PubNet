using System.Diagnostics;
using Vogen;

namespace PubNet.Web.Abstractions.Models;

[ValueObject<string>(conversions: Conversions.SystemTextJson)]
public readonly partial struct Scope
{
	public const string Separator = ":";

	public const string Any = "*";

	public static Scope operator +(Scope left, Scope right)
	{
		Debug.Assert(left.Value != null, nameof(left.Value) + " != null");
		Debug.Assert(right.Value != null, nameof(right.Value) + " != null");

		return From(left.Value + Separator + right.Value);
	}

	public static Scope operator +(Scope left, string right)
	{
		Debug.Assert(left.Value != null, nameof(left.Value) + " != null");

		return From(left.Value + Separator + right);
	}

	public Scope WithAny() => this + Any;

	// public Scope Parent()
	// {
	// 	Debug.Assert(Value != null, nameof(Value) + " != null");
	//
	// 	var index = Value.LastIndexOf(Separator, StringComparison.Ordinal);
	//
	// 	return From(index == -1 ? Any : Value[..index]);
	// }

	public IEnumerable<Scope> Traverse()
	{
		Debug.Assert(Value != null, nameof(Value) + " != null");

		var index = Value.LastIndexOf(Separator, StringComparison.Ordinal);

		if (index == -1)
			yield return From(Any);
		else
		{
			var parent = Value[..index];
			yield return From(parent);

			foreach (var scope in From(Value[..index]).Traverse())
				yield return scope;
		}
	}

	public bool IsAny() => string.Equals(Value, Any, StringComparison.Ordinal);

	public bool IsParentOf(Scope scope)
	{
		Debug.Assert(Value != null, nameof(Value) + " != null");
		Debug.Assert(scope.Value != null, nameof(scope.Value) + " != null");

		var segments = Value.Split(Separator);
		var otherSegments = scope.Value.Split(Separator);

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
}
