using Microsoft.AspNetCore.Identity;

namespace ChineseFamilyRestaurant.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ICollection<Order>? Orders { get; set; }
    }
}
