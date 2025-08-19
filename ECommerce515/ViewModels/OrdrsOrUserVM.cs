using Microsoft.AspNetCore.Mvc.Rendering;

namespace ECommerce515.ViewModels
{
    public class OrdrsOrUserVM
    {
        public Order orders { get; set; } = null!;
        public List<SelectListItem>? applicationUsers { get; set; } 
    }
}
