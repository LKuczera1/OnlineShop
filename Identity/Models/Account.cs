using Identity.Dtos;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Utility.DtoEntity;
using Utility.Enums;

namespace Identity.Models
{
    public class Account :IEntity<AccountDto>
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Email { get; set; }
        [AllowNull]
        public string PhoneNumber { get; set; } = string.Empty;
        [Required]
        public string Address { get; set; } = string.Empty;
        [Required]
        public string City { get; set; } = string.Empty;
        [Required]
        public PrivilegeLevel PriviledgeLevel { get; set; } = PrivilegeLevel.NotAssigned;

        public AccountDto ToDto() => new AccountDto
        {
            UserName = UserName,
            Password = Password,
            Email = Email,
            PhoneNumber = PhoneNumber,
            Address = Address,
            City = City,
            PriviledgeLevel = PriviledgeLevel
        };

        public void FromDto(int id, AccountDto dto)
        {
            Id = id;
            UserName = dto.UserName;
            Password = dto.Password;
            Email = dto.Email;
            PhoneNumber = dto.PhoneNumber;
            Address = dto.Address;
            City = dto.City;
            PriviledgeLevel = PriviledgeLevel;
        }
    }
}

