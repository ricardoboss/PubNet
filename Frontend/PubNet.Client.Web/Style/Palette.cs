using System.Diagnostics;
using MudBlazor.Utilities;

namespace PubNet.Client.Web.Style;

public static class Palette
{
	public static string White => "#ffffff";
	public static string Black => "#000000";

	public static string Blue => "#7ab3fb";
	public static string Red => "#be2b40";
	public static string Gray => "#6e7781";

	#region Color Tools
	internal static string Shade(this string color, int percent)
	{
		Debug.Assert(percent is >= 0 and <= 100, "Percent must be between 0 and 100");
		Debug.Assert(percent % 10 == 0, "Percent must be a multiple of 10");

		// ColorDarken maps -0.5 to 0.5 to -100% to 100%, so we divide by 200 to get a range from 0 to 1
		return new MudColor(color).ColorDarken(percent / 200.0).ToString(MudColorOutputFormats.Hex);
	}

	internal static string Tint(this string color, int percent)
	{
		Debug.Assert(percent is >= 0 and <= 100, "Percent must be between 0 and 100");
		Debug.Assert(percent % 10 == 0, "Percent must be a multiple of 10");

		return new MudColor(color).ColorLighten(percent / 100.0).ToString(MudColorOutputFormats.Hex);
	}
	#endregion
}
