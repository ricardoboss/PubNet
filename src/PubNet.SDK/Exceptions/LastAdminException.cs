namespace PubNet.SDK.Exceptions;

/// <summary>
/// The account cannot be deleted or demoted because it is the last administrator of the instance.
/// </summary>
public class LastAdminException(Exception innerException)
	: PubNetSdkException(
		"This is the only administrator account left. Make someone else an administrator first",
		innerException);
