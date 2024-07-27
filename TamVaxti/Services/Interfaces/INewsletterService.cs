using System.Threading.Tasks;

namespace TamVaxti.Services.Interfaces
{
    public interface INewsletterService
    {
        bool IsEmailSubscribed(string email);
        void SubscribeEmail(string email);
    }
}
