namespace SMS.EPA.Core.Services
{
    using Constants;
    using Fanex.Common;

    public class CryptographyService : ICryptographyService
    {
        public string Encrypt(string plainText)
        {
            string cipherText = string.Empty;

            if (!string.IsNullOrWhiteSpace(plainText))
            {
                cipherText = Cryptography.CfsEnCrypt(Constants.KeyCryptography + Cryptography.CfsEnCrypt(plainText));
            }

            return cipherText;
        }

        public string Decrypt(string cipherText)
        {
            string plainText = string.Empty;

            if (!string.IsNullOrWhiteSpace(cipherText))
            {
                plainText = Cryptography.CfsDeCrypt(Cryptography.CfsDeCrypt(cipherText).Replace(Constants.KeyCryptography, string.Empty));
            }

            return plainText;
        }
    }
}