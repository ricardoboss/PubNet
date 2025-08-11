using PubNet.API.DTO.Errors;

namespace PubNet.API.DTO.Packages.Dart.Spec;

public class DartSuccessDto
{
	public static DartSuccessDto WithMessage(string code, string message)
	{
		return new()
		{
			Success = new()
			{
				Code = code,
				Message = message,
			},
		};
	}

	public required CodeMessageDto Success { get; init; }
}
