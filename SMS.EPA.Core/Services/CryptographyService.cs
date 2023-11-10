namespace SMS.EPA.Core.Services
{
    using Constants;
    using Fanex.Data;

    public class CryptographyService : ICryptographyService
    {
        public string Encrypt(string plainText)
        {
            string cipherText = string.Empty;

            if (!string.IsNullOrWhiteSpace(plainText))
            {
                cipherText = Cryptography.EncryptPassword(Constants.KeyCryptography + Cryptography.EncryptPassword(plainText));
            }

            return cipherText;
        }

        public string Decrypt(string cipherText)
        {
            string plainText = string.Empty;

            if (!string.IsNullOrWhiteSpace(cipherText))
            {
                plainText = Cryptography.DecryptPassword(Cryptography.DecryptPassword(cipherText).Replace(Constants.KeyCryptography, string.Empty));
            }

            return plainText;
        }
    }
}