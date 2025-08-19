using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ECommerce515.Areas.Admin.Controllers
{
      [Area("Admin")]
    public class userController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _dbcontex;

        public userController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext dbcontex)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _dbcontex = dbcontex;
        }

        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users.ToList();
            var userList = new List<userorRoleVM>();

            foreach (var item in users)
            {
                var roles = await _userManager.GetRolesAsync(item);
                userList.Add(new userorRoleVM
                {
                    Id = item.Id,
                    FullName = $"{item.FirstName} {item.LastName}",
                    Email = item.Email,
                    UserName = item.UserName,
                    Roles = roles.ToList(),
                   islocked = item.LockoutEnabled

                });

            }

            return View(userList);
        }

        public async Task<IActionResult> Details(string Id)
        {
            var user = await _userManager.FindByIdAsync(Id);
            if (user is null)
            {
                return NotFound();
            }
            var roleuser = await _userManager.GetRolesAsync(user);
            var models = new userorRoleVM()
            {
                Id = user.Id,
                FullName = user.FirstName + " " + user.LastName,
                Email = user.Email,
                UserName = user.UserName,
                Roles = roleuser.ToList(),
                islocked = user.LockoutEnabled

            };
           
            return View(models);
        }

        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user is null)
            {
                return NotFound();
            }
            await _userManager.DeleteAsync(user);
            return RedirectToAction(nameof(Index), "user");
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Roles =_roleManager.Roles.Select(e => e.Name).ToList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ApplicationUser applicationUser, string Password ,List<string> Roles)
        {
            if (ModelState.IsValid)
            {
                var result = await _userManager.CreateAsync(applicationUser, Password);
               if (result.Succeeded)
               { 
                    if(Roles is not null)
                    {
                        foreach (var item in Roles)
                        {
                            await _userManager.AddToRoleAsync(applicationUser, item);
                        }
                       
                        TempData["success-notification"] = "Add User Successfully";
                        return RedirectToAction(nameof(Index));
                    }

               }
                foreach (var item in result.Errors)
               ModelState.AddModelError(" ", item.Description);
             
            }
            ViewBag.Roles = _roleManager.Roles.Select(e => e.Name).ToList();
            return View(applicationUser);
        }
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user is null)
            {
                return NotFound();
            }
            ViewBag.UserRoles =await _userManager.GetRolesAsync(user);
            ViewBag.Roles = _roleManager.Roles.Select(e => e.Name).ToList();
            return View(user);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(ApplicationUser applicationUser, List<string> Roles)
        {
            var user = await _userManager.FindByIdAsync(applicationUser.Id);
            if (user is null)
            {
                return NotFound();
            }
            user.FirstName = applicationUser.FirstName;
            user.LastName = applicationUser.LastName;
            user.Email = applicationUser.Email;
            user.UserName = applicationUser.UserName;
            var resualt = await _userManager.UpdateAsync(user);
            if (resualt.Succeeded)
            {
                var roleuser = await _userManager.GetRolesAsync(user);
                foreach (var item in roleuser)
                {
                    await _userManager.RemoveFromRoleAsync(user, item);
                }
                if (Roles is not null)
                {
                    foreach (var item in Roles)
                    {
                        await _userManager.AddToRoleAsync(user, item);
                    }
                    TempData["success-notification"] = "update User Successfully";
                    return RedirectToAction(nameof(Index));
                }
            }
            foreach (var item in resualt.Errors)
            {
                ModelState.AddModelError(" ", item.Description);
            }
            ViewBag.Roles = _roleManager.Roles.Select(e => e.Name).ToList();
            return View(applicationUser);
        }

        public async Task<IActionResult> LockUNlock(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user is not null)
            {
                if (user.LockoutEnabled)
                {
                    user.LockoutEnabled = false;
                    user.LockoutEnd = null;
                    TempData["success-notification"] = " unlocked the User";

                }
                else
                {
                    user.LockoutEnabled = true;
                    user.LockoutEnd = DateTime.UtcNow.AddDays(1);
                    TempData["success-notification"] = " locked the User";

                }
                await _userManager.UpdateAsync(user);
            }
            return RedirectToAction(nameof(Index));
        }
         
    }     
}
