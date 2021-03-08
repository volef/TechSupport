using System;
using System.Linq;

namespace RequestSender
{
    internal static class StringGenerator
    {
        private static readonly string chars = " ?!ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        private static readonly Random random = new Random();

        public static string Get(int lenght)
        {
            return new string(Enumerable.Repeat(chars, lenght)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}