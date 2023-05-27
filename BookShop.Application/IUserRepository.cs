using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookShop.Domain;

namespace BookShop.Application
{
    public interface IUserRepository : IGenericRepository<User>
    {
        User GetUserByLogin(string login, string password);
    }
}
