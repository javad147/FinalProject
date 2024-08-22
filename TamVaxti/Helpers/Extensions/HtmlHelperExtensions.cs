using System.Text.RegularExpressions;
namespace TamVaxti.Helpers.Extensions

{
    public static class HtmlHelperExtensions
    {
        public static string StripHtml(this string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                return source;
            }

            return Regex.Replace(source, "<.*?>", string.Empty);
        }
    }
}
