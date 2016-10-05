namespace SMS.EPA.Core.Services
{
    public interface ICryptographyService
    {
        string Encrypt(string plainText);

        string Decrypt(string cipherText);
    }
}