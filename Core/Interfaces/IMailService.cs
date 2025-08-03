// Core/Interfaces/IMailService.cs
namespace Core.Interfaces
{
    public interface IMailService
    {
        void SendVerificationCode(string toEmail, string code);
    }
}
