using System.Text.RegularExpressions;

namespace Domain.Utils;

public static class RegexUtil
{
    #region MATCH

    public static bool HasOnlyNumbers(string sequence)
    {
        return !Regex.IsMatch(sequence, @"^\d$");
    }

    public static bool HasRepeatedCharacters(string sequence)
    {
        return Regex.IsMatch(sequence, @"(\w)*.*\1");
    }

    public static bool IsEmail(string sequence)
    {
        return Regex.IsMatch(sequence, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z");
    }

    public static bool HasSpecialCharacters(string sequence)
    {
        return Regex.IsMatch(sequence, @"(?=.*[!-/:-@[-`{-~])");
    }

    #endregion MATCH
}
