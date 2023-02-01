namespace PubNet.API.Extensions;

public static class ArrayExtensions
{
	public static void Deconstruct<T>(this T[] list, out T first, out T second, out T[] rest)
	{
		first = list.First();
		second = list.Skip(1).First();
		rest = list.Skip(2).ToArray();
	}
}
