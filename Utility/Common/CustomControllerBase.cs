using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Utility.Enums;

namespace Utility.Common
{
    public class CustomControllerBase :ControllerBase
    {
        //public class UserData
        //{
        //    public int? ClientId { get; set; }
        //    public PriviledgeLevel PriviledgeLevel { get; set; }
        //}

        

        protected int? GetUserId()
        {
            var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(value, out var id)) return id;

            return null;
        }

        protected bool IsUserInRole(string role)
        {
            return User.IsInRole(role);
        }

        protected string GetUserRole()
        {
            //First because user claims only single role
            return User.Claims.Where(r => r.Type == ClaimTypes.Role).Select(r => r.Value).First();
        }

        protected PriviledgeLevel GetPrivilegeLevel()
        {
            var role = GetUserRole();

            if(role!=null) return RolesStr.RoleToEnum(role);

            return PriviledgeLevel.NotAssigned;
        }

        protected UserData GetUserData()
        {
            return new UserData(GetUserId(), GetPrivilegeLevel());
        }

        //protected UserData GetUserData()
        //{
        //    return new UserData() { ClientId = GetUserId(), PriviledgeLevel = GetPrivilegeLevel() };
        //}
    }
}
