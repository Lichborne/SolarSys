using System;

namespace Backend
{
    public static class StringExtensions
    {
        public static string Truncate(this string stringy, int length)
        {
            if (length >= stringy.Length)
                return stringy;
            
            return stringy.Substring(0, length);
        }
    }
}