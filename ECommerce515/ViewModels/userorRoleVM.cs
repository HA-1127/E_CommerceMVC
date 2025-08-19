using Microsoft.AspNetCore.Identity;

namespace ECommerce515.ViewModels
{
    public class userorRoleVM
    {

        public string? Id { get; set; } 
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? UserName { get; set; }
        public string? Address { get; set; }
        public string? PhoneNnmber { get; set; }
        public List<string>? Roles { get; set; }
        public bool islocked { get; set; }
    }
}
