using System.Diagnostics.CodeAnalysis;

[SuppressMessage("ReSharper", "CheckNamespace", Justification = "Assembly-level")]
internal static partial class ThisAssembly
{
	public static string MajorMinorPatch { get; } = AssemblyVersion.Split('.').Take(3).Aggregate((a, b) => $"{a}.{b}");
	public static string SemVer { get; } = AssemblyInformationalVersion.Split('+', 2).First();
	public static string GitShortCommitId { get; } = GitCommitId[..7];
}
