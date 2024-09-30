using MudBlazor;

namespace PubNet.Client.Web.Style;

public static class Theme
{
	public static MudTheme Default => new()
	{
		Typography = new()
		{
			Default = new()
			{
				FontFamily = ["system-ui", "-apple-system", "BlinkMacSystemFont", "Segoe UI", "Roboto", "Helvetica", "Arial", "sans-serif", "Apple Color Emoji", "Segoe UI Emoji"],
				FontSize = "1rem",
				FontWeight = 400,
			},
		},

		LayoutProperties = new()
		{
			DefaultBorderRadius = "0.5rem",
		},

		PaletteDark = new()
		{
			Black = Palette.Black,
			White = Palette.White,

			Background = Palette.Gray.Shade(80),
			AppbarBackground = Palette.Gray.Shade(70),

			PrimaryLighten = Palette.Blue,
			Primary = Palette.Blue.Shade(30),
			PrimaryDarken = Palette.Blue.Shade(50),
			PrimaryContrastText = Palette.Blue,

			ErrorLighten = Palette.Red.Tint(30),
			Error = Palette.Red.Tint(10),
			ErrorDarken = Palette.Red,
			ErrorContrastText = Palette.Red.Tint(90),

			DarkLighten = Palette.Gray,
			Dark = Palette.Gray.Shade(20),
			DarkDarken = Palette.Gray.Shade(50),

			TextPrimary = Palette.White,
			TextSecondary = Palette.Gray.Shade(10),

			LinesInputs = Palette.Gray.Shade(10),
		},
	};
}
