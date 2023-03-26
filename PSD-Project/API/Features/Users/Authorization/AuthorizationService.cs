using System.Collections.Generic;
using System.Linq;
using Util.Collections;
using Util.Option;

namespace PSD_Project.API.Features.Users.Authorization
{
    public class AuthorizationService : IAuthorizationService
    {
        private readonly IRolesRepository rolesRepository;
        
        private readonly Dictionary<int, List<Permission>> permissionsTable = new Dictionary<int, List<Permission>>
        {
            [0] = new List<Permission>
            {
                Permission.PlaceOrder,
                Permission.ReadOwnTransactions
            },
            [1] = new List<Permission>
            {
                Permission.HandleOrder,
                Permission.ReadCustomerUserdetails
            },
            [2] = new List<Permission>
            {
                Permission.PlaceOrder,
                Permission.ReadOwnTransactions,
                Permission.HandleOrder,
                Permission.ReadCustomerUserdetails,
                Permission.ReadStaffUserdetails,
                Permission.ReadAllTransactions
            }
        };

        public AuthorizationService(IRolesRepository rolesRepository)
        {
            this.rolesRepository = rolesRepository;
        }

        public Option<Permission> PermissionToRead(int roleId)
        {
            switch (roleId)
            {
                case 0:
                    return Option.Some(Permission.ReadCustomerUserdetails);
                case 1:
                    return Option.Some(Permission.ReadStaffUserdetails);
                default:
                    return Option.None<Permission>();
            }
        }

        public bool RoleHasPermission(int roleId, Permission permission)
        {
            return rolesRepository.GetRole(roleId)
                .Ok()
                .Map(roleDetails => permissionsTable[roleDetails.Id].Contains(permission))
                .OrElse(false);
        }

        public List<Permission> GetPermissions(int roleId)
        {
            return rolesRepository.GetRole(roleId)
                .Ok()
                .Map(roleDetails => roleDetails.Id)
                .Bind(permissionsTable.Get)
                .OrElse(new List<Permission>());
        }
    }
}