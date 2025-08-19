using Microsoft.AspNetCore.Identity;
using Stripe.Climate;

namespace ECommerce515.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Address { get; set; }
        public ICollection<Order> orders { get; set; } = new List<Order>();
       
    }
}
