using System.Diagnostics.CodeAnalysis;

namespace PubNet.Database.Entities;

public static class AotHelper
{
	internal const DynamicallyAccessedMemberTypes DynamicallyAccessedMemberTypes =
		System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.PublicConstructors
		| System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.NonPublicConstructors
		| System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.PublicProperties
		| System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.PublicFields
		| System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.NonPublicProperties
		| System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.NonPublicFields
		| System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.Interfaces;

}
