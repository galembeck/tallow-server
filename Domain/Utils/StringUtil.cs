using Domain.Constants;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

namespace Domain.Utils;

public static class StringUtil
{
    public static string DATEFORMAT_NUMBERS_ONLY = "yyyyMMddHHmmss";

    public static string GetDateAsNumbersOnly(DateTimeOffset? date = null)
    {
        return date == null ? DateTimeOffset.UtcNow.ToString(DATEFORMAT_NUMBERS_ONLY)
            : date.Value.ToString(DATEFORMAT_NUMBERS_ONLY);
    }

    /// <summary>
    /// Format a Date using a string format or one of the <see cref="DateFormatConstants"/> provided. Will use <see cref="DateFormatConstants.ISO_8601"/> if none are provided.
    /// </summary>
    /// <param name="format">The format to be used, can be a custom format or one of the <see cref="DateFormatConstants"/>, <see cref="DateFormatConstants.ISO_8601"/> if null.</param>
    /// <param name="date">The date to be formated, <see cref="DateTimeOffset.UtcNow"/> if null.</param>
    /// <returns></returns>
    public static string GetDateFormated(string format = null, DateTimeOffset? date = null)
    {
        format ??= DateFormatConstants.ISO_8601;

        return date == null ? DateTimeOffset.UtcNow.ToString(format)
            : date.Value.ToString(format);
    }

    public static string GetMonthInPortugueseByNumber(int month)
    {
        #pragma warning disable CS8603 // Possible null reference return.
        return month switch
        {
            1 => "Janeiro",
            2 => "Fevereiro",
            3 => "Março",
            4 => "Abril",
            5 => "Maio",
            6 => "Junho",
            7 => "Julho",
            8 => "Agosto",
            9 => "Setembro",
            10 => "Outubro",
            11 => "Novembro",
            12 => "Dezembro",
            _ => null
        };
        #pragma warning restore CS8603 // Possible null reference return.
    }

    public static string GetObjectAsJson(object obj)
    {
        if (obj != null)
        {
            JsonSerializerOptions options = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
            };

            return JsonSerializer.Serialize(obj, options);
        }
        else
            return null;
    }

    public static object ConvertJsontoObject<T>(string json)
    {
        JsonSerializerOptions options = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
        };

        try
        {
            return JsonSerializer.Deserialize<T>(json, options);
        }
        catch
        {
            return null;
        }
    }
    public static T DeserializeJson<T>(string json) => JsonSerializer.Deserialize<T>(json);
    public static string SerializeJson<T>(T json) => JsonSerializer.Serialize<T>(json);
    public static string GenerateRandom(int length = 10)
    {
        StringBuilder sb = new StringBuilder();
        string auxString;

        while (sb.Length < length)
        {
            auxString = Convert.ToBase64String(Guid.NewGuid().ToByteArray())
                .Replace("+", String.Empty)
                .Replace("=", String.Empty)
                .Replace("/", String.Empty);

            sb.Append((length - sb.Length > auxString.Length) ? auxString : auxString.Substring(0, length - sb.Length));
        }

        return sb.ToString();
    }

    private static int NextInt(int min, int max)
    {
        byte[] buffer = new byte[4];
        var result = RandomNumberGenerator.GetInt32(BitConverter.ToInt32(buffer, 0));
        return new Random(result).Next(min, max);
    }

    private static int CalculateVerificationDigit(string seed)
    {
        int sum = 0;
        int leftOver = 0;
        int[] multipliers = new int[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

        var iFinish = multipliers.Count();
        var iStart = iFinish - seed.Count();

        for (int i = iStart; i < iFinish; i++)
            sum += int.Parse(seed[i - iStart].ToString()) * multipliers[i];

        leftOver = sum % 11;

        if (leftOver < 2)
            leftOver = 0;
        else
            leftOver = 11 - leftOver;

        return leftOver;
    }

    public static string MD5Hash(string raw)
    {
        // step 1, calculate MD5 hash from input
        MD5 md5 = MD5.Create();
        Byte[] inputBytes = Encoding.UTF8.GetBytes(raw);
        Byte[] hash = md5.ComputeHash(inputBytes);

        return BitConverter.ToString(hash).Replace("-", String.Empty);
    }

    public static string SHA512(string inputString)
    {
        using (SHA512 sha521 = System.Security.Cryptography.SHA512.Create())
            return GetStringFromHash(sha521.ComputeHash(Encoding.UTF8.GetBytes(inputString)));

    }

    // Don't remove
    private static string GetStringFromHash(byte[] hash)
    {
        var result = new StringBuilder();
        for (int i = 0; i < hash.Length; i++)
            result.Append(hash[i].ToString("X2"));
        return result.ToString();
    }

    public static bool IsValidCPF(string cpf)
    {
        try
        {
            if (String.IsNullOrEmpty(cpf))
                return false;


            cpf = StringUtil.Slugify(cpf, isRemoving: true);
            cpf = cpf.Replace(" ", "");
            int[] multiplicador1 = new int[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = new int[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

            string tempCpf;
            string digito;
            int soma;
            int resto;

            if (string.IsNullOrWhiteSpace(cpf))
                return false;

            cpf = cpf.Trim().Replace(".", "").Replace("-", "");
            if (cpf.Length != 11)
                return false;

            tempCpf = cpf.Substring(0, 9);
            soma = 0;

            for (int i = 0; i < 9; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];
            resto = soma % 11;

            if (resto < 2) resto = 0;
            else resto = 11 - resto;

            digito = resto.ToString();
            tempCpf += digito;
            soma = 0;

            for (int i = 0; i < 10; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];
            resto = soma % 11;

            if (resto < 2) resto = 0;
            else resto = 11 - resto;

            return cpf.EndsWith(digito += resto.ToString());
        }
        catch
        {
            return false;
        }
    }

    public static bool IsValidCNPJ(string cnpj)
    {
        try
        {
            cnpj = StringUtil.Slugify(cnpj, isRemoving: true);
            cnpj = cnpj.Replace(" ", "");
            Int64 number;
            bool isNumber = Int64.TryParse(cnpj, out number);

            if (String.IsNullOrEmpty(cnpj) || !isNumber)
            {
                return false;
            }


            int[] multiplicador1 = new int[12] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = new int[13] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

            int soma;
            int resto;
            string digito;
            string tempCnpj;
            cnpj = cnpj.Trim();
            cnpj = cnpj.Replace(".", "").Replace("-", "").Replace("/", "");
            if (cnpj.Length != 14)
                return false;
            tempCnpj = cnpj.Substring(0, 12);
            soma = 0;
            for (int i = 0; i < 12; i++)
                soma += int.Parse(tempCnpj[i].ToString()) * multiplicador1[i];
            resto = (soma % 11);
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;
            digito = resto.ToString();
            tempCnpj = tempCnpj + digito;
            soma = 0;
            for (int i = 0; i < 13; i++)
                soma += int.Parse(tempCnpj[i].ToString()) * multiplicador2[i];
            resto = (soma % 11);
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;
            digito = digito + resto.ToString();
            return cnpj.EndsWith(digito);
        }
        catch
        {
            return false;
        }
    }

    public static Boolean IsValidCellphone(string cellphone)
    {
        if (String.IsNullOrEmpty(cellphone))
        {
            return false;
        }

        Boolean response = true;
        for (int i = 0; i < cellphone.Count() - 1; i++)
        {
            var count = 0;
            for (int j = 0; j < cellphone.Count(); j++)
            {
                if (cellphone[i] == cellphone[j])
                {
                    count++;
                }
            }
            if (count > 8)
            {
                response = false;
                break;
            }
        }
        return response;
    }

    public static bool IsValidRG(string rg)
    {
        if (rg?.Length > 14)
            return false;

        return true;
    }

    public static bool IsValidCNH(string cnh)
    {
        bool isValid = false;
        cnh = StringUtil.Slugify(cnh, isRemoving: true);
        cnh = cnh.Replace(" ", "");
        Int64 number;
        bool isNumber = Int64.TryParse(cnh, out number);

        if (isNumber)
        {
            if (cnh.Length == 11 && cnh != new string('1', 11))
            {
                var dsc = 0;
                var v = 0;
                for (int i = 0, j = 9; i < 9; i++, j--)
                {
                    v += (Convert.ToInt32(cnh[i].ToString()) * j);
                }

                var vl1 = v % 11;
                if (vl1 >= 10)
                {
                    vl1 = 0;
                    dsc = 2;
                }

                v = 0;
                for (int i = 0, j = 1; i < 9; ++i, ++j)
                {
                    v += (Convert.ToInt32(cnh[i].ToString()) * j);
                }

                var x = v % 11;
                var vl2 = (x >= 10) ? 0 : x - dsc;

                isValid = vl1.ToString() + vl2.ToString() == cnh.Substring(cnh.Length - 2, 2);
            }
        }

        return isValid;
    }

    public static bool IsValidRNE(string rne)
    {
        if (rne?.Length > 10)
        {
            return false;
        }
        return true;
    }

    public static bool IsValidDocument(string document)
    {
        if (IsValidCPF(document) || IsValidCNPJ(document))
        {
            return true;
        }

        return false;
    }

    public static string RemoveSpecialCharacters(string input, char[] ignoreList = null)
    {
        if (String.IsNullOrEmpty(input))
            return input;

        List<char> ignoreCharList = new List<char>();

        if (ignoreList != null)
        {
            ignoreCharList.AddRange(ignoreList);
        }

        StringBuilder sb = new StringBuilder();
        foreach (char c in input)
        {
            if ((c >= '0' && c <= '9')
                || (c >= 'A' && c <= 'Z')
                || (c >= 'a' && c <= 'z')
                || (ignoreCharList.Contains(c)))
            {
                sb.Append(c);
            }
        }
        return sb.ToString();
    }

    public static string CapStringLength(string input, int cap)
    {
        string result = input;

        if (String.IsNullOrEmpty(input))
            return input;

        if (input.Length > cap)
            result = input.Substring(0, cap);

        return result;
    }

    public static string FormatCNPJ(string CNPJ)
        => Convert.ToUInt64(CNPJ).ToString(@"00\.000\.000\/0000\-00");

    public static string FormatCPF(string CPF)
        => Convert.ToUInt64(CPF).ToString(@"000\.000\.000\-00");

    public static string FormatMoney(int value)
        => string.Format("R${0:#.00}", Convert.ToDecimal(value) / 100);

    public static string FormatMoney(int? value)
        => string.Format("R${0:#.00}", Convert.ToDecimal(value) / 100);

    public static double GetMoneyValue(int value)
        => ((double)value / 100);

    public static double GetMoneyValue(int? value)
        => ((double)value / 100);

    public static string DocumentWithoutFormat(string documentString)
        => documentString.Replace(".", string.Empty).Replace("-", string.Empty).Replace("/", string.Empty);

    public static void AddStringValueToDictionary(Dictionary<String, String> dictionary, string key, string value)
    {
        if (dictionary.ContainsKey(key))
        {
            dictionary[key] = dictionary[key] + ", " + value;
        }
        else
        {
            dictionary[key] = value;
        }
    }

    public static Boolean BooleanFromString(string value)
    {
        return value.ToUpper() switch
        {
            "TRUE" => true,
            "FALSE" => false,
            _ => false,
        };
    }

    public static Boolean IsParseableBooleanFromString(string value)
    {
        if (String.IsNullOrEmpty(value))
            return false;

        return value.ToUpper() switch
        {
            "TRUE" => true,
            "FALSE" => true,
            _ => false,
        };
    }

    public static string Slugify(string data, Boolean isAcceptingExtended = false, Boolean isRemoving = true, string keepChars = "")
    {
        if (String.IsNullOrEmpty(data))
            return data;

        string result = data;

        string from = "ãàáäâẽèéëêìíïîõòóöôùúüûñçÃÀÁÄÂẼÈÉËÊÌÍÏÎÕÒÓÖÔÙÚÜÛÑÇ";
        string to = "aaaaaeeeeeiiiiooooouuuuncAAAAAEEEEEIIIIOOOOOUUUUNC";
        string valid = "abcdefghijklmnopqrstuvwxyz0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ ";
        string validExtended = "abcdefghijklmnopqrstuvwxyz0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ -=_+|!@#$%&*()[]'\"/,.:;?\n\r";

        for (int i = 0, l = from.Length; i < l; i++)
        {
            result = result.Replace(from.Substring(i, 1), to.Substring(i, 1));
        }

        // remove invalid chars
        foreach (char c in result)
        {
            if (isAcceptingExtended ? !validExtended.Contains(c) : !valid.Contains(c))
            {
                if (!keepChars.Contains(c))
                {
                    result = isRemoving ? result.Replace(c.ToString(), String.Empty) : result.Replace(c, '-');
                }
            }
        }

        return result;
    }

    public static Boolean IsValidGuid(string value)
    {
        if (!Guid.TryParse(value, out _))
            return false;
        else
            return true;
    }

    public static string GetMaxLogString(string text)
    {
        if (text.Length > 3990)
            text = text.Substring(0, 3990);

        return text;
    }

    public static string GetMaxLogJobString(string text)
    {
        if (text.Length > 2000)
            text = text.Substring(0, 2000);

        return text;
    }

    public static string GetInnerException(System.Exception ex)
    {
        System.Exception realerror = ex;
        while (realerror.InnerException != null)
            realerror = realerror.InnerException;

        return realerror.Message;
    }

    public static List<string> MemoryStreamToStringList(MemoryStream memoryStream)
    {
        List<String> result = new List<String>();

        if (memoryStream == null)
            return result;

        string ln = null;

        StreamReader sr = new StreamReader(memoryStream);

        while ((ln = sr.ReadLine()) != null)
        {
            result.Add(ln);
        }

        return result;
    }

    public static string RemoveInvoiceBarCodeVerificationDigit(string fullBarCode)
    {
        string barCode;

        if (fullBarCode != " - - - ")
        {
            var barCodeSplit = fullBarCode.Split(" ");
            barCodeSplit[0] = barCodeSplit[0].Remove(11);
            barCodeSplit[1] = barCodeSplit[1].Remove(11);
            barCodeSplit[2] = barCodeSplit[2].Remove(11);
            barCodeSplit[3] = barCodeSplit[3].Remove(11);

            barCode = String.Join("", barCodeSplit[0], barCodeSplit[1], barCodeSplit[2], barCodeSplit[3]);
        }
        else
        {
            barCode = fullBarCode;
        }

        return barCode;
    }

    /// <summary>
    /// Concatena a string com hífen.
    /// </summary>
    /// <param name="strings"></param>
    public static string ConcatenateStringWithHyphen(List<String> strings)
    {
        return String.Join(" - ", strings);
    }

    /// <summary>
    /// Concatena a string com nova linha.
    /// </summary>
    /// <param name="strings"></param>
    public static string ConcatenateStringWithNewLine(List<String> strings)
    {
        return String.Join(Environment.NewLine, strings);
    }

    public static int GetTheLastDayOfMonth(int month)
    {

        switch (month)
        {
            case 1:
            case 2:
                if (DateTime.IsLeapYear(DateTime.Now.Year)) return 29;
                return 28;
            case 3:
            case 5:
            case 7:
            case 8:
            case 10:
            case 12:
                return 31;
            case 4:
            case 6:
            case 9:
            case 11: return 30;
            default: return 30;
        }
    }

    /// <summary>
    /// Remove acentos e caracteres especiais.
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static string RemoveAccents(string text)
    {
        if (string.IsNullOrEmpty(text))
            return text;

        var textr = new System.Text.StringBuilder();

        for (int i = 0; i < text.Length; i++)
        {
            char c = text[i];
            switch (c)
            {
                // Casos para minúsculas
                case 'ã':
                case 'á':
                case 'à':
                case 'â':
                case 'ä':
                    textr.Append('a');
                    break;
                case 'é':
                case 'è':
                case 'ê':
                case 'ë':
                    textr.Append('e');
                    break;
                case 'í':
                case 'ì':
                case 'ï':
                    textr.Append('i');
                    break;
                case 'õ':
                case 'ó':
                case 'ò':
                case 'ö':
                case 'ô':
                    textr.Append('o');
                    break;
                case 'ú':
                case 'ù':
                case 'ü':
                    textr.Append('u');
                    break;
                case 'ç':
                    textr.Append('c');
                    break;

                // Casos para maiúsculas
                case 'Ã':
                case 'Á':
                case 'À':
                case 'Â':
                case 'Ä':
                    textr.Append('A');
                    break;
                case 'É':
                case 'È':
                case 'Ê':
                case 'Ë':
                    textr.Append('E');
                    break;
                case 'Í':
                case 'Ì':
                case 'Ï':
                    textr.Append('I');
                    break;
                case 'Õ':
                case 'Ó':
                case 'Ò':
                case 'Ö':
                case 'Ô':
                    textr.Append('O');
                    break;
                case 'Ú':
                case 'Ù':
                case 'Ü':
                    textr.Append('U');
                    break;

                case '-':
                case ',':
                case '.':
                case ':':
                case ';':
                case 'º':
                case '\'':
                case '<':
                case '>':
                case '!':
                case '@':
                case '#':
                case '$':
                case '%':
                case '^':
                case '&':
                case '*':
                case '+':
                case '=':
                case '?':
                case '/':
                case '´':
                case '`':
                case '~':
                case '\\':
                case '(':
                case ')':
                    // Não adicione nada para ignorar esses caracteres
                    break;
                default:
                    textr.Append(c);
                    break;
            }
        }
        return textr.ToString().Trim();
    }

    /// <summary>
    /// Delimita o stackTrace para a necessidade do log.s
    /// </summary>
    /// <param name="ex"></param>
    /// <param name="maxFrames"></param>
    /// <returns>StackTrace delimitado</returns>
    public static string GetSimplifiedStackTrace(System.Exception ex, int maxFrames)
    {
        if (ex is null)
            return "Exception is null.";

        if (maxFrames <= 0)
            return "Max frames must be greater than 0.";

        try
        {
            var stackTrace = new StackTrace(ex, true); // Obtém o stack trace da exceção
            var stackFrames = stackTrace.GetFrames(); // Obtém os frames do stack trace

            if (stackFrames is null || stackFrames.Length == 0)
                return "No stack trace available.";

            var simplifiedTrace = new StringBuilder();
            int framesAdded = 0;

            for (int i = 0; i < stackFrames.Length; i++)
            {
                var frame = stackFrames[i];
                if (frame is null)
                    continue;

                var method = frame.GetMethod();
                if (method is null)
                    continue;

                var methodName = $"{method.DeclaringType?.FullName
                                    .Replace('+', '.')
                                    .Replace("<", string.Empty)
                                    .Replace(">", " ").Split(" ")[0]}";

                // Obtém os parâmetros do método
                var parameters = method.GetParameters();
                var parameterList = string.Join(", ", parameters.Select(p => $"{p.ParameterType.Name} {p.Name}"));

                // Obtém o número da linha (se disponível)
                var lineNumber = frame.GetFileLineNumber();

                // Adiciona o frame ao stack trace
                simplifiedTrace.AppendLine($"at {methodName}()");

                framesAdded++;

                // Para se atingir o número máximo de frames ou encontrar "ControllerActionInvoker"
                if (framesAdded >= maxFrames || methodName.Contains("ControllerActionInvoker"))
                    break;
            }

            return simplifiedTrace.ToString();
        }
        catch (System.Exception innerEx)
        {
            // Em caso de erro ao processar o stack trace, retorna uma mensagem de erro segura
            return $"Failed to generate simplified stack trace: {innerEx.Message}";
        }
    }

    public static async Task<string> SafeReadContentAsync(HttpResponseMessage response)
    {
        return response?.Content != null
            ? await response.Content.ReadAsStringAsync()
            : "Response null";
    }

    public static string CsatUrlTitle(string url)
    {
        char separator = '/';
        if (string.IsNullOrEmpty(url)) return string.Empty;

        var trimmed = url!.TrimEnd(separator);
        if (trimmed.Length == 0) return string.Empty;

        int idx = trimmed.LastIndexOf(separator);
        return idx < 0 ? string.Empty : trimmed[(idx + 1)..];
    }

    public static string MaskEmail(string email, int keepLeft = 2, int keepRight = 1, char maskChar = '*', bool maskDomain = false)
    {
        if (string.IsNullOrWhiteSpace(email)) return string.Empty;
        var at = email.IndexOf('@');
        if (at <= 0 || at == email.Length - 1) return new string(maskChar, Math.Min(email.Length, 6));

        var local = email.Substring(0, at);
        var domain = email.Substring(at + 1);

        // local: preserva início/fim e mascara o resto
        keepLeft = Math.Clamp(keepLeft, 0, local.Length);
        keepRight = Math.Clamp(keepRight, 0, local.Length - keepLeft);
        var maskCount = Math.Max(0, local.Length - keepLeft - keepRight);

        var maskedLocal =
            local.Substring(0, keepLeft) +
            new string(maskChar, maskCount) +
            (keepRight > 0 ? local.Substring(local.Length - keepRight) : string.Empty);

        if (!maskDomain)
            return $"{maskedLocal}@{domain}";

        var parts = domain.Split('.');
        if (parts.Length == 1) return $"{maskedLocal}@{new string(maskChar, Math.Max(1, parts[0].Length - 1))}";

        for (int i = 0; i < parts.Length - 1; i++)
        {
            if (parts[i].Length > 1)
                parts[i] = parts[i][0] + new string(maskChar, parts[i].Length - 1);
            else
                parts[i] = maskChar.ToString();
        }
        var maskedDomain = string.Join(".", parts);
        return $"{maskedLocal}@{maskedDomain}";
    }
    public static string ToHex(ReadOnlySpan<byte> bytes)
    {
        char[] c = new char[bytes.Length * 2];
        int i = 0;
        foreach (var b in bytes)
        {
            c[i++] = (char)((b >> 4) + ((b >> 4) < 10 ? '0' : ('A' - 10)));
            c[i++] = (char)((b & 0xF) + ((b & 0xF) < 10 ? '0' : ('A' - 10)));
        }
        return new string(c);
    }

    public static byte[] DecodeBase64(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            throw new ArgumentException();

        var s = input.Trim();
        var comma = s.IndexOf(',');
        if (s.StartsWith("data:", StringComparison.OrdinalIgnoreCase) && comma > -1)
            s = s.Substring(comma + 1);

        s = s.Replace("\r", "").Replace("\n", "").Replace(" ", "");
        s = s.Replace('-', '+').Replace('_', '/');
        switch (s.Length % 4)
        {
            case 2: s += "=="; break;
            case 3: s += "="; break;
        }

        return Convert.FromBase64String(s);
    }

    /// <summary>
    /// Apaga dados sensiveis do usuario no RequestBody para registrar em log.
    /// </summary>
    /// <param name="requestBody"></param>
    /// <returns>RequestBody sem dados sensiveis</returns>
    public static string SanitizeRequestBody(string requestBody)
    {
        try
        {
            // Converte o corpo da requisição para um objeto JSON
            var jsonObject = JsonNode.Parse(requestBody);

            if (jsonObject is not null)
            {
                // Verifica e mascara o campo "recaptchaToken" se existir
                if (jsonObject["recaptchaToken"] is not null)
                    jsonObject["recaptchaToken"] = "***";

                // Verifica e mascara o campo "RecaptchaToken" se existir
                if (jsonObject["RecaptchaToken"] is not null)
                    jsonObject["RecaptchaToken"] = "***";

                // Verifica e mascara o campo "password" se existir
                if (jsonObject["password"] is not null)
                    jsonObject["password"] = "***";

                // Verifica e mascara o campo "Password" se existir
                if (jsonObject["Password"] is not null)
                    jsonObject["Password"] = "***";

                // Verifica e mascara o campo "senha" se existir
                if (jsonObject["senha"] is not null)
                    jsonObject["senha"] = "***";

                // Verifica e mascara o campo "Senha" se existir
                if (jsonObject["Senha"] is not null)
                    jsonObject["Senha"] = "***";

                // Retorna o JSON sanitizado como string
                return jsonObject.ToJsonString();
            }
        }
        catch (JsonException)
        {
            // Se ocorrer uma exceção ao analisar o JSON, continue e retorne o corpo original
            return requestBody;
        }

        // Retorna o corpo original se não for um JSON válido ou se não houver alterações
        return requestBody;
    }

    /// <summary>
    /// Apaga dados sensiveis relacionados a token na mensagem do log.
    /// </summary>
    /// <param name="message"></param>
    /// <returns>message sem dados sensiveis</returns>
    public static string SanitizeTokenLog(string message)
    {
        var regex = new Regex(@"(Token:\s*)(\S+)", RegexOptions.IgnoreCase);
        return regex.Replace(message, "$1***");
    }

    /// <summary>
    /// Criação de uma chave unica ItemId para informar na Matera
    /// </summary>
    /// <param name="contract"></param>
    /// <param name="dueDate"></param>
    /// <param name="paidAmount"></param>
    /// <param name="installment"></param>
    /// <returns></returns>
    public static string BuildItemId(string contract, DateTime dueDate, decimal paidAmount, int? installment)
    {
        string contractNorm = (contract ?? string.Empty).Trim();
        string datePart = dueDate.ToString("yyyyMMdd");
        long cents = (long)decimal.Round(paidAmount * 100m, 0, MidpointRounding.AwayFromZero);
        int parcela = installment ?? 0;

        string canonical = $"{contractNorm}|{datePart}|{cents}|{parcela}";
        byte[] hash = SHA256.HashData(Encoding.UTF8.GetBytes(canonical));
        return StringUtil.ToHex(hash.AsSpan(0, 10));
    }
}
