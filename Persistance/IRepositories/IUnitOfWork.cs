using System.Threading.Tasks;

namespace dotnet.boilerplate.Persistance.IRepositories
{
    public interface IUnitOfWork
    {
        Task<bool> CompleteAsync();
    }
}