using System.Collections.Generic;
using Util.Option;

namespace PSD_Project.API.Features.Users.Authorization
{
    public interface IAuthorizationService
    {
        Option<Role> RoleOfId(int roleId);
        bool RoleHasPermission(Role role, Permission permission);
        bool RoleHasPermission(int roleId, Permission permission);
        List<Permission> GetPermissions(int roleId);
    }
}