using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookShop.Domain;
using BookShop.Application;
using Microsoft.EntityFrameworkCore;

namespace BookShop.Infrastructure.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        private readonly ApplicationContext _context;
        public UserRepository(ApplicationContext context) : base(context)
        {
            _context = context;
        }

        public User GetUserByLogin(string login, string password)
        {
            return _context.Users.Where(p => p.Login == login && p.Password == password).Include(p => p.Role).FirstOrDefault();
        } 
    }
}
