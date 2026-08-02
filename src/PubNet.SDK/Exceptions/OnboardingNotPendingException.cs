namespace PubNet.SDK.Exceptions;

public class OnboardingNotPendingException(Exception innerException)
	: PubNetSdkException("This instance has already been set up", innerException);
