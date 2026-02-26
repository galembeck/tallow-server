namespace Domain.Utils;

public static class CellPhoneUtil
{
    public static bool CheckDDDNumberIsValid(string cellPhone)
    {
        if (CellPhoneContainsParenthesis(cellPhone))
        {
            var numberWithoutParentheses = cellPhone.Replace("(", "").Replace(")", "");
            cellPhone = numberWithoutParentheses;
        }
        var dicitionaryDDDNumber = InitialiazeDictionary();
        var dddNumber = cellPhone.Substring(0, 2);
        return dicitionaryDDDNumber.ContainsKey(dddNumber);

    }

    private static IDictionary<string, int> InitialiazeDictionary()
    {
        return new Dictionary<string, int>()
            {
                // Centro-Oeste
                {"61", 61 },
                {"62", 62 },
                {"64", 64 },
                {"65", 65 },
                {"66", 66 },
                {"67", 67 },

                //Nordeste
                {"82", 82 },
                {"71", 71 },
                {"73", 73 },
                {"74", 74 },
                {"75", 75 },
                {"77", 77 },
                {"85", 85 },
                {"88", 88 },
                {"98", 98 },
                {"99", 99 },
                {"83", 83 },
                {"81", 81 },
                {"87", 87 },
                {"86", 86 },
                {"89", 89 },
                {"84", 84 },
                {"79", 79 },

                //Norte
                {"68", 68 },
                {"96", 96 },
                {"92", 92 },
                {"97", 97 },
                {"91", 91 },
                {"93", 93 },
                {"94", 94 },
                {"69", 69 },
                {"95", 95 },
                {"63", 63 },

                //Sudeste
                {"27", 27 },
                {"28", 28 },
                {"31", 31 },
                {"32", 32 },
                {"33", 33 },
                {"34", 34 },
                {"35", 35 },
                {"37", 37 },
                {"38", 38 },
                {"21", 21 },
                {"22", 22 },
                {"24", 24 },
                {"11", 11 },
                {"12", 12 },
                {"13", 13 },
                {"14", 14 },
                {"15", 15 },
                {"16", 16 },
                {"17", 17 },
                {"18", 18 },
                {"19", 19 },

                //Sul
                {"41", 41 },
                {"42", 42 },
                {"43", 43 },
                {"44", 44 },
                {"45", 45 },
                {"46", 46 },
                {"51", 51 },
                {"53", 53 },
                {"54", 54 },
                {"55", 55 },
                {"47", 47 },
                {"48", 48 },
                {"49", 49 }
            };
    }
    private static bool CellPhoneContainsParenthesis(string phoneNumber)
    {
        return phoneNumber.Contains("(") && phoneNumber.Contains(")");
    }
}
