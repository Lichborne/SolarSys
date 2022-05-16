using System;
using System.Text;
using System.Security.Cryptography;

// code taken from https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.hashalgorithm.computehash?view=net-6.0

namespace Backend
{
    public static class Security
    {
        // Returns the SHA256 hash of the string
        public static string Hash(string source)
        {
            using (SHA256 sHA256Hasher = SHA256.Create())
            {
                return Hash(sHA256Hasher, source);
            }
        }

        private static string Hash(HashAlgorithm hashAlgorithm, string source)
        {
            byte[] sourceBytes = Encoding.UTF8.GetBytes(source);
            byte[] hashBytes = hashAlgorithm.ComputeHash(sourceBytes);

            StringBuilder hashTextBuilder = new StringBuilder();
            foreach (byte b in hashBytes)
            {
                string byteAsHexString = b.ToString("x2");
                hashTextBuilder.Append(byteAsHexString);
            }

            return hashTextBuilder.ToString();
        }
    }
}