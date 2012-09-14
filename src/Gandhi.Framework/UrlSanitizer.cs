using System.Text.RegularExpressions;

namespace Gandhi.Framework
{
	public static class UrlSanitizer
	{
		private static readonly Regex InvalidCharacters = new Regex(@"[^a-zA-Z0-9-_.~]+", RegexOptions.Compiled);
		public static string SanitizeUrl(this string str)
		{
			return InvalidCharacters.Replace(str, "-");
		}
	}
}