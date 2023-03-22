using System.Collections.Generic;
using System.Linq;
using Util.Collections;

namespace PSD_Project.API.Features.Users.Authorization
{
    public class AuthorizationService : IAuthorizationService
    {
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
            [Role.Admin] = new List<Permission>
            {
                Permission.HandleOrder,
                Permission.ReadCustomerUserdetails,
                Permission.ReadStaffUserdetails
            }
        };

        public bool RoleHasPermission(Role role, Permission permission)
        {
            return permissionsTable.Get(role)
                .Map(permissions => permissions.Contains(permission))
                .OrElse(false);
        }

        public List<Permission> GetPermissions(Role role)
        {
            return permissionsTable.Get(role)
                .OrElse(new List<Permission>());
        }
    }
}