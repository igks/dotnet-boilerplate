using System.Threading.Tasks;
using dotnet.boilerplate.Models;

namespace dotnet.boilerplate.Persistance.IRepositories
{
    public interface IAuthRepository
    {
        Task<User> Login(string email, string password);
        void Register(User user, string password);
    }
}