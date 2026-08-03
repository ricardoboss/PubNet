namespace PubNet.SDK.Exceptions;

/// <summary>
/// The account cannot be deleted because it is the last administrator of the instance.
/// </summary>
public class LastAdminException(Exception innerException)
	: PubNetSdkException(
		"This is the only administrator account left. Make someone else an administrator before deleting it",
		innerException);
