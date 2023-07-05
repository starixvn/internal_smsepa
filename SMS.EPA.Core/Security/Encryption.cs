namespace SMS.EPA.Core.Security
{
    using System;
    using System.Security.Cryptography;
    using System.Text;
    using Enumerations;

    public static class Encryption
    {
        public static string ComputeHash(HashProvider hashAlgorithm, string plainText, int saltSize = 8)
        {
            // Random salt
            byte[] salt = Random.RngCrypto(saltSize);
            return ComputeHash(hashAlgorithm, plainText, salt);
        }

        public static bool CompareStringToHash(HashProvider hashAlgorithm, string plainText, string encrypt, int saltSize = 8)
        {
            byte[] encryptBuffer = Convert.FromBase64String(encrypt);
            int encryptLength = encryptBuffer.Length;

            // Pluck the salt size off of the end of the hash
            byte[] saltBuffer = new byte[saltSize];
            Array.Copy(encryptBuffer, encryptLength - saltSize, saltBuffer, 0, saltSize);

            string hash = ComputeHash(hashAlgorithm, plainText, saltBuffer);
            return string.Equals(encrypt, hash);
        }

        private static HashAlgorithm GetHashProvider(HashProvider hashAlgorithm)
        {
            HashAlgorithm hash;
            switch (hashAlgorithm)
            {
                case HashProvider.Sha1:
                    hash = new SHA1Managed();
                    break;

                case HashProvider.Sha256:
                    hash = new SHA256Managed();
                    break;

                case HashProvider.Sha384:
                    hash = new SHA384Managed();
                    break;

                case HashProvider.Sha512:
                    hash = new SHA512Managed();
                    break;

                default:
                    hash = new MD5CryptoServiceProvider();
                    break;
            }

            return hash;
        }

        private static string ComputeHash(HashProvider hashAlgorithm, string plainText, byte[] salt)
        {
            string textToCompute = plainText ?? string.Empty;

            //// Convert plain text into a byte array
            byte[] data = Encoding.UTF8.GetBytes(textToCompute);
            int dataLength = data.Length;

            //// Random salt
            int saltLength = salt.Length;

            //// Append the salt to the end of the plainText
            byte[] dataSalt = new byte[dataLength + saltLength];

            //// Copy both the data and salt into the new array
            Array.Copy(data, dataSalt, dataLength);
            Array.Copy(salt, 0, dataSalt, dataLength, saltLength);

            //// Compute hash value of our plain text with appended salt.
            byte[] encrypt = GetHashProvider(hashAlgorithm).ComputeHash(dataSalt);
            int encryptLength = encrypt.Length;

            //// Append the salt to the end of the encrypt
            byte[] encryptSalt = new byte[encryptLength + saltLength];
            Array.Copy(encrypt, encryptSalt, encryptLength);
            Array.Copy(salt, 0, encryptSalt, encryptLength, saltLength);

            return Convert.ToBase64String(encryptSalt);
        }
    }
}