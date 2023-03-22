using System.Collections.Generic;
using Util.Collections;

namespace PSD_Project.API.Features.Users.Authorization
{
    public class AuthorizationService : IAuthorizationService
    {
        private readonly Dictionary<Role, HashSet<Permission>> permissionsTable = new Dictionary<Role, HashSet<Permission>>
        {
            [Role.Customer] = new HashSet<Permission>
            {
                Permission.PlaceOrder,
                Permission.ReadOwnTransactions
            },
            [Role.Staff] = new HashSet<Permission>
            {
                Permission.HandleOrder,
                Permission.ReadCustomerUserdetails
            },
            [Role.Admin] = new HashSet<Permission>
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
    }
}