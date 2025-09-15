using NuGet.Common;
using System.Data;
using Utility.Enums;

namespace Identity.Dtos
{
    public class AuthResponseDto
    {
        public string Token { get; set; }
        public DateTime ExpiresAt { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public PriviledgeLevel PriviledgeLevel { get; set; }
    }
}