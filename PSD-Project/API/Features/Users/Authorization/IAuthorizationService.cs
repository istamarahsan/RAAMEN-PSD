using System.Collections.Generic;
using Util.Option;

namespace PSD_Project.API.Features.Users.Authorization
{
    public interface IAuthorizationService
    {
        Option<Permission> PermissionToRead(int roleId);
        bool RoleHasPermission(int roleId, Permission permission);
        List<Permission> GetPermissions(int roleId);
    }
}