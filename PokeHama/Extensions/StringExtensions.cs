using System.Globalization;
using System.Text;

namespace PokeHama.Extensions;

public static class StringExtensions
{
	/// <summary>
	/// Replaces most common used UTF-8 characters in the French language by their ascii version.
	/// </summary>
	/// <param name="this">The string to be formatted.</param>
	/// <returns>A new string without any UTF-8 character.</returns>
	public static string ToAscii(this string @this)
	{
		return @this.ToLower().Replace("é", "e").Replace("è", "e").Replace("ê", "e").Replace("ë", "e")
							  .Replace("à", "a").Replace("ù", "u")
							  .Replace("ô", "o").Replace("ö", "o")
							  .Replace("ï", "i").Replace("î", "i");
	}
}