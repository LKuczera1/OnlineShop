using Identity.Dtos;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Utility.DtoEntity;

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

        public AccountDto ToDto() => new AccountDto
        {
            UserName = UserName,
            Password = Password,
            Email = Email,
            PhoneNumber = PhoneNumber,
            Address = Address,
            City = City,
        };
    }
}
