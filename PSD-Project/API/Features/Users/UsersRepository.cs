using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using PSD_Project.EntityFramework;
using Util.Option;
using Util.Try;

namespace PSD_Project.API.Features.Users
{
    public class UsersRolesRepository : IUserRepository, IRolesRepository
    {
        private readonly Raamen db = new Raamen();
        
        public Try<User, Exception> GetUser(int userId)
        {
            try
            {
                return db.Users.FirstOrDefault(u => u.Id == userId)
                    .ToOption()
                    .OrErr(() => new Exception())
                    .Map(ConvertUserModel);
            }
            catch (Exception e)
            {
                return Try.Err<User, Exception>(e);
            }
            
        }

        public Try<List<User>, Exception> GetUsers()
        {
            try
            {
                var users = db.Users.ToList().Select(ConvertUserModel).ToList();
                return Try.Of<List<User>, Exception>(users);
            }
            catch (Exception e)
            {
                return Try.Err<List<User>, Exception>(e);
            }
            
        }

        public Try<List<User>, Exception> GetUsersWithRole(int roleId)
        {
            try
            {
                var users = db.Users
                    .Where(user => user.Roleid == roleId)
                    .ToList()
                    .Select(ConvertUserModel)
                    .ToList();
                return Try.Of<List<User>, Exception>(users);
            }
            catch (Exception e)
            {
                return Try.Err<List<User>, Exception>(e);
            }
        }

        public Try<List<User>, Exception> GetUsersWithUsername(string username)
        {
            try
            {
                var users = db.Users.Where(user => user.Username == username)
                    .ToList()
                    .Select(ConvertUserModel)
                    .ToList();
                return Try.Of<List<User>, Exception>(users);
            }
            catch (Exception e)
            {
                return Try.Err<List<User>, Exception>(e);
            }
        }

        public Try<RoleDetails, Exception> GetRole(int roleId)
        {
            try
            {
                var role = db.Roles.Find(roleId);
                if (role == null) throw new Exception();
                return Try.Of<RoleDetails, Exception>(ConvertRoleModel(role));
            }
            catch (Exception e)
            {
                return Try.Err<RoleDetails, Exception>(e);
            }
        }
        
        public Try<List<RoleDetails>, Exception> GetRoles()
        {
            try
            {
                var roles = db.Roles.ToList().Select(ConvertRoleModel).ToList();
                return Try.Of<List<RoleDetails>, Exception>(roles);
            }
            catch (Exception e)
            {
                return Try.Err<List<RoleDetails>, Exception>(e);
            }
        }

        public Try<User, Exception> AddNewUser(string username, string email, string password, string gender, int roleId)
        {
            var foundRole = db.Roles.FirstOrDefault(role => role.id == roleId);
            
            if (foundRole == null) 
                return Try.Err<User, Exception>(new ArgumentException("Role with that id does not exist"));
            
            var newId = db.Users.Select(users => users.Id).DefaultIfEmpty(0).Max() + 1;

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
                db.SaveChanges();
            }
            catch (Exception e)
            {
                return Try.Err<User, Exception>(e);
            }

            return Try.Of<User, Exception>(new User(newId, username, email, password, gender, new RoleDetails(foundRole.id, foundRole.name)));
        }

        public Try<User, Exception> UpdateUser(int userId, string username, string email, string gender)
        {
            var user = db.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null) return Try.Err<User, Exception>(new ArgumentException());
            user.Username = username;
            user.Email = email;
            user.Gender = gender;
            try
            {
                db.SaveChanges();
            }
            catch (Exception e)
            {
                return Try.Err<User, Exception>(e);
            }
            return Try.Of<User, Exception>(ConvertUserModel(user));
        }

        private User ConvertUserModel(PSD_Project.EntityFramework.User user) => 
            new User(
                user.Id, 
                user.Username, 
                user.Email, 
                user.Password,
                user.Gender,
                new RoleDetails(
                    user.Role.id,
                    user.Role.name));

        private RoleDetails ConvertRoleModel(Role role) => new RoleDetails(role.id, role.name);
    }
}