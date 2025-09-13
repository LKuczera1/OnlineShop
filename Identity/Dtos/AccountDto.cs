using Identity.Models;
using Utility.DtoEntity;

namespace Identity.Dtos
{

    public class AccountDto :IDto<Account>
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string City { get; set; }

        public Account ToEntity(int id) => new Account
        {
            Id = id,
            UserName = UserName,
            Password = Password,
            Email = Email,
            PhoneNumber = PhoneNumber,
            Address = Address,
            City = City,
        };
    }
}
