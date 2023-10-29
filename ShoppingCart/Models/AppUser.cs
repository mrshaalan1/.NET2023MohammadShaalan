using Microsoft.AspNetCore.Identity;

namespace ShoppingCart.Models
{
    public class AppUser : IdentityUser
    {
        public string Occupation { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
    }
}
