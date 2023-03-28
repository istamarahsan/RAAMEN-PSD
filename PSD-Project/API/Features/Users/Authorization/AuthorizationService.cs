using System.Collections.Generic;
using System.Linq;
using Util.Collections;
using Util.Option;

namespace PSD_Project.API.Features.Users.Authorization
{
    public class AuthorizationService : IAuthorizationService
    {
        private enum Role
        {
            Member,
            Staff,
            Admin
        }
        
        private readonly IRolesRepository rolesRepository;

        private readonly Dictionary<int, Role> rolesTable = new Dictionary<int, Role>()
        {
            [0] = Role.Member,
            [1] = Role.Staff,
            [2] = Role.Admin
        };

        private readonly Dictionary<Role, HashSet<Permission>> permissionsTable = new Dictionary<Role, HashSet<Permission>>
        {
            [Role.Member] = new HashSet<Permission>
            {
                Permission.ViewRamen,
                Permission.PlaceOrders,
                Permission.ViewOwnTransactions,
                Permission.UpdateProfile
            },
            [Role.Staff] = new HashSet<Permission>
            {
                Permission.ViewCustomerUserdetails,
                Permission.CreateRamen,
                Permission.UpdateRamen,
                Permission.DeleteRamen,
                Permission.UpdateProfile,
                Permission.ViewOrders,
                Permission.HandleOrders
            },
            [Role.Admin] = new HashSet<Permission>
            {
                Permission.ViewCustomerUserdetails,
                Permission.ViewStaffUserdetails,
                Permission.ViewAllTransactions,
                Permission.ViewTransactionReports
            }
        };

        public AuthorizationService(IRolesRepository rolesRepository)
        {
            this.rolesRepository = rolesRepository;

            permissionsTable[Role.Admin] = permissionsTable[Role.Admin]
                .Union(permissionsTable[Role.Member])
                .Union(permissionsTable[Role.Staff])
                .ToHashSet();
        }

        public Option<Permission> PermissionToRead(int roleId)
        {
            return rolesTable.Get(roleId).Bind(PermissionToRead);
        }

        public bool RoleHasPermission(int roleId, Permission permission)
        {
            return rolesRepository.GetRole(roleId)
                .Ok()
                .Bind(roleDetails => rolesTable.Get(roleDetails.Id))
                .Map(role => permissionsTable[role].Contains(permission))
                .OrElse(false);
        }

        public List<Permission> GetPermissions(int roleId)
        {
            return rolesRepository.GetRole(roleId)
                .Ok()
                .Bind(roleDetails => rolesTable.Get(roleDetails.Id))
                .Bind(permissionsTable.Get)
                .Map(set => set.ToList())
                .OrElse(new List<Permission>());
        }
        
        private Option<Permission> PermissionToRead(Role role)
        {
            switch (role)
            {
                case Role.Member:
                    return Option.Some(Permission.ViewCustomerUserdetails);
                case Role.Staff:
                    return Option.Some(Permission.ViewStaffUserdetails);
                case Role.Admin:
                default:
                    return Option.None<Permission>();
            }
        }
    }
}