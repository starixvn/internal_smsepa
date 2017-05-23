namespace SMS.EPA.Services.Interfaces
{
    using Model.Email;

    public interface IBaseMailService
    {
        void SendEmail(MailModel email);

        string ReadTextFile(string filePath);
    }
}