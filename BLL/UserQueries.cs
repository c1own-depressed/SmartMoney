﻿using DAL;
using Org.BouncyCastle.Crypto.Generators;

namespace BLL
{
    public class UserQueries
    {
        static public User AddUser(string username, string email, string password)
        {
            // Install-Package BCrypt.Net
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt(12));

            User newUser = new User
            {
                Username = username,
                Email = email,
                HashedPassword = hashedPassword,
                LightTheme = true,
                Language = "ua",
                Currency = "uah",
            };

            Queries.AddUser(newUser);

            return newUser;
        }

        static public User GetUser(int userId)
        {
            return Queries.GetUserById(userId);
        }

        static public User GetUserByUsername(string username)
        {
            return Queries.GetUserByUsername(username);
        }

        static public int CheckCredentials(string username, string password)
        {
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt(12));

            User user = GetUserByUsername(username);
            if (user.HashedPassword == hashedPassword)
            {
                return user.Id;
            }
            else
            {
                return 0;
            }
        }
    }
}