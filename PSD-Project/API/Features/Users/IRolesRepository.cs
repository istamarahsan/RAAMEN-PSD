using System;
using System.Collections.Generic;
using Util.Try;

namespace PSD_Project.API.Features.Users
{
    public interface IRolesRepository
    {
        Try<RoleDetails, Exception> GetRole(int roleId);
        Try<List<RoleDetails>, Exception> GetRoles();
    }
}