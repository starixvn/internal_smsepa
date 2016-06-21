namespace SMS.EPA.Core.Services.Interfaces
{
    public interface IBaseMailService
    {
        void SendEmail(MailModel email);

        string ReadTextFile(string filePath);
    }
}