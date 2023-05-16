using System;
using System.Collections.Generic;
using System.Linq;
using Util.Collections;
using Util.Option;

namespace PSD_Project.API.Features.Users.Authorization
{
    public class AuthorizationService : IAuthorizationService
    {
        private readonly IRolesRepository rolesRepository;
        
        private readonly Dictionary<Role, List<Permission>> permissionsTable = new Dictionary<Role, List<Permission>>
        {
            [Role.Customer] = new List<Permission>
            {
                Permission.PlaceOrder,
                Permission.ReadOwnTransactions
            },
            [Role.Staff] = new List<Permission>
            {
                Permission.HandleOrder,
                Permission.ReadCustomerUserdetails
            },
            [Role.Admin] = Enum.GetValues(typeof(Permission))
                .Cast<Permission>()
                .ToList()
        };

        public AuthorizationService(IRolesRepository rolesRepository)
        {
            this.rolesRepository = rolesRepository;
        }

        public Option<Role> RoleOfId(int roleId) => ParseRoleId(roleId);

        public bool RoleHasPermission(Role role, Permission permission)
        {
            return permissionsTable.Get(role)
                .Map(permissions => permissions.Contains(permission))
                .OrElse(false);
        }

        public bool RoleHasPermission(int roleId, Permission permission)
        {
            return rolesRepository.GetRole(roleId)
                .Ok()
                .Map(roleDetails => roleDetails.Id)
                .Bind(ParseRoleId)
                .Map(role => RoleHasPermission(role, permission))
                .OrElse(false);
        }

        public List<Permission> GetPermissions(int roleId)
        {
            return rolesRepository.GetRole(roleId)
                .Ok()
                .Map(roleDetails => roleDetails.Id)
                .Bind(ParseRoleId)
                .Bind(permissionsTable.Get)
                .OrElse(new List<Permission>());
        }
        
        private Option<Role> ParseRoleId(int roleId)
        {
            switch (roleId)
            {
                case 2:
                    return Option.Some(Role.Admin);
                case 1:
                    return Option.Some(Role.Staff);
                case 0:
                    return Option.Some(Role.Customer);
                default:
                    return Option.None<Role>();
            }
        }
    }
}