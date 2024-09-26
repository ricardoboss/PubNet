namespace PubNet.API.DTO.Packages.Dart.Spec;

public class DartSuccessDto
{
	public static DartSuccessDto WithMessage(string message)
	{
		return new()
		{
			Success = new()
			{
				Message = message,
			},
		};
	}

	public required DartMessageDto Success { get; init; }
}
