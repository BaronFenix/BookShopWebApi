using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BookShop.Domain;


namespace BookShop.Infrastructure
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Author> Authors { get; set; } = null!;
        public DbSet<Book> Books { get; set; } = null!;
        public DbSet<Busket> Buskets { get; set; } = null!;
        public DbSet<Genre> Genres { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<Price> Prices { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<BookAuthor> BookAuthors { get; set; } = null!;
        public DbSet<BookGenre> BookGenres { get; set; } = null!;
        public DbSet<Role> Roles { get; set; } = null!;



        // Если базы нет то создает ее, если база есть но пустая то создает таблицы в ней. Если все есть то ок.
        public ApplicationContext() => Database.EnsureCreated();


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string path = Directory.GetCurrentDirectory() + "\\BookShopBD.mdf";
            optionsBuilder.UseSqlServer($"Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename={path};Integrated Security=True;Connect Timeout=30");
        }

    }
}
