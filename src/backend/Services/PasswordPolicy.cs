using System.Text.RegularExpressions;

namespace DevFlow.Api.Services;

public static class PasswordPolicy
{
    private static readonly Regex ComplexityRegex = new(
        @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z\d]).{8,}$",
        RegexOptions.Compiled);

    public static bool IsValid(string password) => ComplexityRegex.IsMatch(password);

    public const string PolicyDescription =
        "Password must be at least 8 characters and include an uppercase letter, a lowercase letter, a number, and a special character.";
}
