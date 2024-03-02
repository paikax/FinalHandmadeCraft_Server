using System;
using System.Linq;
using Data.Context;
using Data.Entities.User;
using Microsoft.EntityFrameworkCore;
using Common.Constants;

namespace Service.InitDB
{
    public class InitDb : IDbInit
    {
        private readonly AppDbContext _db;
        
        public InitDb(AppDbContext db)
        {
            _db = db;
        }

        public void InitDB()
        {
            try
            {
                if (_db.Database.GetPendingMigrations().Any()) 
                {
                    _db.Database.Migrate();
                    Console.WriteLine("[DB]---> Migrations applied successfully.");
                }
                else
                {
                    Console.WriteLine("[DB]-------> No pending migrations.");
                }

                // Create admin account if it doesn't exist
                if (!_db.Users.Any(u => u.Role == StringEnums.Roles.AdminRole))
                {
                    var adminUser = new User
                    {
                        Email = "paika2060@gmail.com",
                        FirstName = "Phong",
                        LastName = "Mai",
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword("Phong123"),
                        Role = StringEnums.Roles.AdminRole,
                    };
                    
                    _db.Users.Add(adminUser);
                    _db.SaveChanges();
                    Console.WriteLine("[DB]-------> Admin account created successfully.");
                }
                else
                {
                    Console.WriteLine("[DB]-------> Admin account already exists.");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("[DB]------->Error applying migrations: " + e.Message);
                throw;
            }
        }
    }
}