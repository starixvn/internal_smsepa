namespace SMS.EPA.Services
{
    using Model.Email;

    public interface IBaseMailService
    {
        void SendEmail(MailModel email);

        string ReadTextFile(string filePath);
    }
}