using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using PSD_Project.EntityFramework;
using Util.Option;

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

        public async Task AddNewUserAsync(string username, string email, string password, string gender, int roleId)
        {
            if (!await db.Roles.Select(role => role.id).ContainsAsync(roleId)) 
                throw new ArgumentException("Role with that id does not exist");
            
            db.Users.Add(new PSD_Project.EntityFramework.User
            {
                Id = db.Users.Select(users => users.Id).DefaultIfEmpty(0).Max() + 1,
                Username = username,
                Email = email,
                Gender = gender,
                Password = password,
                Roleid = roleId
            });

            await db.SaveChangesAsync();
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