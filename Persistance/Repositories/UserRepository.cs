using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using dotnet.boilerplate.Helpers;
using dotnet.boilerplate.Helpers.Params;
using dotnet.boilerplate.Models;
using dotnet.boilerplate.Persistance.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace dotnet.boilerplate.Persistance.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User> Login(string email, string password)
        {
            var user = await _context.User.FirstOrDefaultAsync(u => u.Email == email);

            if (user != null)
            {
                if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                {
                    user = null;
                }
            }

            return user;
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != passwordHash[i]) return false;
                }
                return true;
            }
        }

        public async Task<IEnumerable<User>> GetAll()
        {
            var users = await _context.User.ToListAsync();
            return users;
        }

        public async Task<User> GetById(int id)
        {
            return await _context.User.FindAsync(id);
        }

        public async Task<User> GetByEmail(string email)
        {
            var user = await _context.User.FirstOrDefaultAsync(u => u.Email == email);
            return user;
        }

        public async Task<PagedList<User>> GetPaged(UserParams userParams)
        {
            var users = _context.User.AsQueryable();

            if (!string.IsNullOrEmpty(userParams.Firstname))
            {
                users = users.Where(u => u.Firstname.Contains(userParams.Firstname));
            }

            var columnsMap = new Dictionary<string, Expression<Func<User, object>>>()
            {
                ["name"] = u => u.Firstname
            };

            users = users.ApplyOrdering(userParams, columnsMap);

            return await PagedList<User>
                .CreateAsync(users, userParams.PageNumber, userParams.PageSize);
        }

        public void Add(User user, string password)
        {
            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            _context.User.Add(user);
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        public void Update(User user)
        {
            _context.User.Attach(user);
            _context.Entry(user).State = EntityState.Modified;
        }

        public void Remove(User user)
        {
            _context.Remove(user);
        }
    }
}