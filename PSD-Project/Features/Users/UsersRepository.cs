using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using PSD_Project.EntityFramework;
using Util.Option;
using Util.Try;

namespace PSD_Project.Features.Users
{
    public class UsersRepository : IUsersRepository
    {
        private readonly Raamen db = new Raamen();
        
        public async Task<Option<User>> GetUserAsync(int userId)
        {
            return (await db.Users.Where(user => user.Id == userId).FirstOrDefaultAsync())
                .ToOption()
                .Map(ConvertModel);
        }

        public async Task<List<User>> GetUsersAsync()
        {
            var users = await db.Users.ToListAsync();
            return users.Select(ConvertModel).ToList();
        }

        public async Task<List<User>> GetUsersWithRoleAsync(int roleId)
        {
            var usersWithRoleId = await db.Users.Where(user => user.Roleid == roleId).ToListAsync();
            return usersWithRoleId.AsEnumerable()
                .Select(ConvertModel)
                .ToList();
        }

        public async Task<Option<User>> GetUserWithUsernameAsync(string username)
        {
            var usersWithUsername = await db.Users.Where(user => user.Username == username).ToListAsync();
            return usersWithUsername.AsEnumerable()
                .FirstOrDefault()
                .ToOption()
                .Map(ConvertModel);
        }

        public async Task<Try<User, Exception>> AddNewUserAsync(string username, string email, string password, string gender, int roleId)
        {
            var foundRole = await db.Roles.Where(role => role.id == roleId).FirstOrDefaultAsync();
            
            if (foundRole == null) 
                return Try.Err<User, Exception>(new ArgumentException("Role with that id does not exist"));
            
            var newId = await db.Users.Select(users => users.Id).DefaultIfEmpty(0).MaxAsync() + 1;
            

            db.Users.Add(new PSD_Project.EntityFramework.User
            {
                Id = newId,
                Username = username,
                Email = email,
                Gender = gender,
                Password = password,
                Roleid = roleId
            });

            try
            {
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return Try.Err<User, Exception>(e);
            }

            return Try.Of<User, Exception>(new User(newId, username, email, password, gender, new Role(foundRole.id, foundRole.name)));
        }

        public async Task<Try<User, Exception>> UpdateUserAsync(int userId, string username, string email, string gender)
        {
            var user = await db.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return Try.Err<User, Exception>(new ArgumentException());
            user.Username = username;
            user.Email = email;
            user.Gender = gender;
            try
            {
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return Try.Err<User, Exception>(e);
            }
            return Try.Of<User, Exception>(ConvertModel(user));
        }

        private User ConvertModel(PSD_Project.EntityFramework.User user) => 
            new User(
                user.Id, 
                user.Username, 
                user.Email, 
                user.Password,
                user.Gender,
                new Role(
                    user.Role.id,
                    user.Role.name));
    }
}