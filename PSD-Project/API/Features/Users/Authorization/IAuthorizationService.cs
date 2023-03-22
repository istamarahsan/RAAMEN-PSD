using System.Collections.Generic;

namespace PSD_Project.API.Features.Users.Authorization
{
    public interface IAuthorizationService
    {
        bool RoleHasPermission(Role role, Permission permission);
        List<Permission> GetPermissions(Role role);
    }
}