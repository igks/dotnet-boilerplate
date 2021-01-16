using System;
using System.Threading.Tasks;
using dotnet.boilerplate.Persistance.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace dotnet.boilerplate.Persistance.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private AppDbContext context;

        public UnitOfWork(AppDbContext context)
        {
            this.context = context;
        }

        public async Task<bool> CompleteAsync()
        {
            int saveResult = 0;
            try
            {
                saveResult = await context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine(ex);
                saveResult = 0;
            }
            return saveResult > 0;
        }
    }
}