using PubNet.API.Abstractions;

namespace PubNet.API.Exceptions;

public class SetupAlreadyCompleteException() : ApiException("setup-already-complete", "The setup is already complete.",
	StatusCodes.Status418ImATeapot);
