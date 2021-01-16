using System.Collections.Generic;
using System.Threading.Tasks;
using dotnet.boilerplate.Helpers;
using dotnet.boilerplate.Helpers.Params;
using dotnet.boilerplate.Models;

namespace dotnet.boilerplate.Persistance.IRepositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAll();
        Task<User> GetById(int id);
        Task<User> GetByEmail(string email);
        void Add(User user, string password);
        void Update(User user);
        void Remove(User user);
        Task<PagedList<User>> GetPaged(UserParams userParams);
    }
}