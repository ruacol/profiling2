using System;
using System.Text.RegularExpressions;

namespace Profiling2.Infrastructure.Util
{
    public static class IDNumber
    {
        static string[] regexes = new string[] 
            { 
                @"^\d{2,3}\.?\d{3}[/\.]?[A-za-z]{1}$",  // FARDC
                @"^\d{1}-?\d{2}-?\d{2}-?\d{5}-?\d{2}$",  // FARDC (current)
                @"^SM\d{4,6}$",  // FARDC (former)
                @"^\d{13}$",  // PNC
                @"^\d{3,4}\.?\d{3}$",  // PNC civilian
            };

        public static bool IsRecognised(string s)
        {
            if (!string.IsNullOrEmpty(s))
            {
                string[] ids = s.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                bool result = true;
                foreach (string id in ids)
                    result &= IsSingleRecognised(id.Trim());
                return result;
            }
            return false;
        }

        static bool IsSingleRecognised(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                foreach (string regex in IDNumber.regexes)
                {
                    Match match = Regex.Match(id, regex);
                    if (match.Success)
                        return true;
                }
            }
            return false;
        }
    }
}
