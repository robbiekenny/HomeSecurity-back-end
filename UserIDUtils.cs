using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace homesecurityService
{
    public class UserIDUtils
    {
        public static string RemoveSpecialCharacters(string s)
        {
            string result = "";
            for (int i = 0; i < s.Length; i++)
            {
                if (Char.IsLetterOrDigit(s[i]) && !Char.IsWhiteSpace(s[i])
                    && !Char.IsSymbol(s[i]) && !Char.IsPunctuation(s[i]))
                {
                    result += s[i];
                }
            }
            return result;
        }
    }
}