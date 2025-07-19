using System.Text.RegularExpressions;

namespace Drosy.Domain.Shared.System.Validation.Patterns
{
    public static class RegexPatterns
    {
        public static readonly Regex Phone = new(@"^\+?[1-9]\d{1,14}$"); // E.164
        public static readonly Regex Email = new(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");
    }
}
