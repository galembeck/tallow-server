using Domain.Constants;
using Domain.Data.Entities;
using Domain.Enumerators;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;

namespace Domain.Utils;

public class SecurityUtil
{
    public static string PBKDF2Hash(string input)
    {
        var hashed = KeyDerivation.Pbkdf2(
            input,
            Encoding.ASCII.GetBytes("<TODO_BASE>"),
            KeyDerivationPrf.HMACSHA512,
            10000,
            32);

        string hex = "";
        foreach (var c in hashed)
            hex += ((int)c).ToString("x");

        return hex;
    }

    public static bool GetPasswordStrength(string password)
    {
        int score = 0;
        if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(password.Trim()))
            return false;

        if (HasMinimumLength(password, 5))
            score++;

        if (HasMinimumLength(password, 8))
            score++;

        if (HasUpperCaseLetter(password) && HasLowerCaseLetter(password))
            score++;

        if (HasDigit(password))
            score++;

        if (HasSpecialChar(password))
            score++;

        if ((PasswordStrength)score != PasswordStrength.VeryStrong)
            return false;

        return true;
    }

    /// <summary>
    /// Sample password policy implementation:
    /// - minimum 8 characters
    /// - at lease one UC letter
    /// - at least one LC letter
    /// - at least one non-letter char (digit OR special char)
    /// </summary>
    /// <returns></returns>
    public static bool IsStrongPassword(string password)
    {
        return HasMinimumLength(password, 8)
            && HasUpperCaseLetter(password)
            && HasLowerCaseLetter(password)
            && (HasDigit(password) || HasSpecialChar(password));
    }

    /// <summary>
    /// Sample password policy implementation following the Microsoft.AspNetCore.Identity.PasswordOptions standard.
    /// </summary>
    public static bool IsValidPassword(string password, int requiredLength, int requiredUniqueChars, bool requireNonAlphanumeric,
        bool requireLowercase, bool requireUppercase, bool requireDigit)
    {
        if (!HasMinimumLength(password, requiredLength))
            return false;

        if (!HasMinimumUniqueChars(password, requiredUniqueChars))
            return false;

        if (requireNonAlphanumeric && !HasSpecialChar(password))
            return false;

        if (requireLowercase && !HasLowerCaseLetter(password))
            return false;

        if (requireUppercase && !HasUpperCaseLetter(password))
            return false;

        if (requireDigit && !HasDigit(password))
            return false;

        return true;
    }

    public static string EncryptData(string data)
    {
        if (!string.IsNullOrEmpty(data))
        {
            byte[] Results;
            UTF8Encoding UTF8 = new UTF8Encoding();
            var TDESAlgorithm = TripleDES.Create();
            var md5 = System.Security.Cryptography.MD5.Create();
            var key = md5.ComputeHash(UTF8.GetBytes(Constant.Settings.CriptBankKey));
            TDESAlgorithm.Key = key;
            TDESAlgorithm.Mode = CipherMode.ECB;
            TDESAlgorithm.Padding = PaddingMode.PKCS7;
            byte[] DataToEncrypt = UTF8.GetBytes(data);
            try
            {
                var Encryptor = TDESAlgorithm.CreateEncryptor();
                Results = Encryptor.TransformFinalBlock(DataToEncrypt, 0, DataToEncrypt.Length);
            }
            finally
            {
                TDESAlgorithm.Clear();
            }
            return Convert.ToBase64String(Results);

        }
        else
            return data;
    }

    public static string DecryptData(string data)
    {
        if (!string.IsNullOrEmpty(data))
        {
            byte[] Results;
            var md5 = System.Security.Cryptography.MD5.Create();
            UTF8Encoding UTF8 = new UTF8Encoding();
            var TDESAlgorithm = TripleDES.Create();
            TDESAlgorithm.Key = md5.ComputeHash(UTF8.GetBytes(Constant.Settings.CriptBankKey));
            TDESAlgorithm.Mode = CipherMode.ECB;
            TDESAlgorithm.Padding = PaddingMode.PKCS7;
            byte[] DataToDecrypt = Convert.FromBase64String(data);
            try
            {
                var decryptor = TDESAlgorithm.CreateDecryptor();
                Results = decryptor.TransformFinalBlock(DataToDecrypt, 0, DataToDecrypt.Length);
            }

            finally
            {
                TDESAlgorithm.Clear();
            }
            return UTF8.GetString(Results);

        }
        else
            return data;
    }



    #region Helper Methods

    public static bool HasMinimumLength(string password, int minLength)
    {
        return password.Length >= minLength;
    }

    public static bool HasMinimumUniqueChars(string password, int minUniqueChars)
    {
        return password.Distinct().Count() >= minUniqueChars;
    }

    /// <summary>
    /// Returns TRUE if the password has at least one digit
    /// </summary>
    public static bool HasDigit(string password)
    {
        return password.Any(c => char.IsDigit(c));
    }

    /// <summary>
    /// Returns TRUE if the password has at least one special character
    /// </summary>
    public static bool HasSpecialChar(string password)
    {
        return RegexUtil.HasSpecialCharacters(password);
    }

    /// <summary>
    /// Returns TRUE if the password has at least one uppercase letter
    /// </summary>
    public static bool HasUpperCaseLetter(string password)
    {
        return password.Any(c => char.IsUpper(c));
    }

    /// <summary>
    /// Returns TRUE if the password has at least one lowercase letter
    /// </summary>
    public static bool HasLowerCaseLetter(string password)
    {
        return password.Any(c => char.IsLower(c));
    }

    /// <summary>
    /// Metodo de descriptografia especifico para fintech - Por motivos de seguranca foi criado uma nova senha de criptografia/descriptografia
    /// </summary>
    /// <param name="Message"></param>
    /// <returns></returns>
    public static string DescriptografarMarketplace(string Message)
    {
        byte[] Results;
        var UTF8 = new UTF8Encoding();
        var TDESAlgorithm = TripleDES.Create();
        var md5 = System.Security.Cryptography.MD5.Create();
        var key = md5.ComputeHash(UTF8.GetBytes(Constant.Settings.FintechPass));
        TDESAlgorithm.Key = key;
        TDESAlgorithm.Mode = CipherMode.ECB;
        TDESAlgorithm.Padding = PaddingMode.PKCS7;
        byte[] DataToDecrypt = Convert.FromBase64String(Message);
        try
        {
            ICryptoTransform Decryptor = TDESAlgorithm.CreateDecryptor();
            Results = Decryptor.TransformFinalBlock(DataToDecrypt, 0, DataToDecrypt.Length);
        }
        finally
        {
            TDESAlgorithm.Clear();
        }
        return UTF8.GetString(Results);
    }

    public static UserSecurityInfo GetSecurityInfo(HttpRequest request)
    {
        // Get security informations
        IPHostEntry ipEntry = Dns.GetHostEntry(Dns.GetHostName());
        IPAddress[] addr = ipEntry.AddressList;
        var ip = addr[addr.Length - 1].ToString();

        NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
        string macAddress = string.Empty;
        foreach (NetworkInterface adapter in nics)
        {
            if (macAddress == String.Empty)
            {
                IPInterfaceProperties properties = adapter.GetIPProperties();
                macAddress = adapter.GetPhysicalAddress().ToString();
            }
        }

        var browser = request.Headers["User-Agent"].ToString();

        UserSecurityInfo securityInfo = new UserSecurityInfo()
        {
            Ip = ip,
            MacAdress = macAddress,
            Browser = browser,
        };

        return securityInfo;
    }

    #endregion Helper Methods
}