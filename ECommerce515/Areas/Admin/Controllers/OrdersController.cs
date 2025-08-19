using ECommerce515.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace ECommerce515.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrdersController : Controller
    {
        private readonly IOrderRepository _orderRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _dbContext;

        public OrdersController(IOrderRepository orderRepository , UserManager<ApplicationUser> userManager ,ApplicationDbContext dbContext)
        {
            _orderRepository = orderRepository;
            _userManager = userManager;
            _dbContext = dbContext;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var orders =await _orderRepository.GetAsync(includes: [e => e.ApplicationUser]);
            return View(orders);
        }
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var order =await _orderRepository.GetOneAsync(e => e.Id == id, includes: [e => e.ApplicationUser]);
            return View(order);
        } 
        public async Task<IActionResult> Delete(int id)
        {
            var order = await _orderRepository.GetOneAsync(e => e.Id == id);
            if (order is not null)
            {
                _orderRepository.Delete(order);
              await  _orderRepository.CommitAsync();
                TempData["success-notification"] = " Delete Order Successfully";
                return RedirectToAction(nameof(Index));

            }
            return NotFound();
        }
    
        public async Task<IActionResult> Shipped(int id)
        {
            var order = await _orderRepository.GetOneAsync(e => e.Id == id, includes: [e => e.ApplicationUser]);
            if (order is null)
            {
                TempData["success-notification"] = "  Order not found";
                return RedirectToAction(nameof(Index));
            }
            order.OrderStatus = OrderStatus.shipped;
            await _orderRepository.CommitAsync();

            TempData["success-notification"] = "  Order Status Shipped";
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Completed(int id)
        {
            var order = await _orderRepository.GetOneAsync(e => e.Id == id, includes: [e => e.ApplicationUser]);
            if (order is null)
            {
                TempData["success-notification"] = "  Order not found";
                return RedirectToAction(nameof(Index));
            }
            order.OrderStatus = OrderStatus.completed;
          await  _orderRepository.CommitAsync();
            TempData["success-notification"] = "  Order Status Completed";
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Canceled(int id)
        {
            var order = await _orderRepository.GetOneAsync(e => e.Id == id, includes: [e => e.ApplicationUser]);
            if (order is null)
            {
                TempData["success-notification"] = "  Order not found";
                return RedirectToAction(nameof(Index));
            }
            order.OrderStatus = OrderStatus.canceled;
            await _orderRepository.CommitAsync();

            TempData["success-notification"] = "  Order Status canceled";
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Create()
        {
            //OrdrsOrUserVM ordrsOrUserVM = new()
            //{
            //    applicationUsers = _userManager.Users.Select(e => new SelectListItem()
            //    {
            //        Value = e.Id,
            //        Text = e.UserName
            //    }).ToList(),
            //    orders = new()

            //};
            ViewBag.applicationUsers = _userManager.Users.Select(e => new SelectListItem()
            {
                Value = e.Id,
                Text = e.UserName
            }).ToList();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Order order)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.applicationUsers = _userManager.Users.Select(e => new SelectListItem()
                {
                    Value = e.Id,
                    Text = e.UserName
                }).ToList();
                return View(order);
             
            }
            await _orderRepository.CreateAsync(order);
            await _orderRepository.CommitAsync();
            TempData["success-notification"] = "Successfull add Orders";
            return RedirectToAction(nameof(Index));
            

        }

        public async Task<IActionResult> Edit(int id)
        {
            var order =await _orderRepository.GetOneAsync(e => e.Id == id);
            if (order is not null)
            {
                //ViewBag.applicationUsers = _userManager.Users.Select(e => new SelectListItem()
                //{
                //    Value = e.Id,
                //    Text = e.UserName
                //}).ToList();
                //return View(order);
                var application =_userManager.Users.ToList();
                OrdrsOrUserVM ordrsOrUserVM = new()
                {
                    applicationUsers =application.Select(e => new SelectListItem()
                    {
                        Value = e.Id,
                        Text = e.UserName
                    }).ToList(),
                    orders = order

                };
                return View(ordrsOrUserVM);
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> Edit(OrdrsOrUserVM ordrsOrUserVM)
        {
            if (!ModelState.IsValid)
            {
                //ViewBag.applicationUsers = _userManager.Users.Select(e => new SelectListItem()
                //{
                //    Value = e.Id,
                //    Text = e.UserName
                //}).ToList();
                //return View(order);
                var application = _userManager.Users.ToList();
                OrdrsOrUserVM ordrsOrUserVM1 = new()
                {
                    applicationUsers = application.Select(e => new SelectListItem()
                    {
                        Value = e.Id,
                        Text = e.UserName
                    }).ToList(),
                    orders = new()

                };
                return View(ordrsOrUserVM1);
            }
            _orderRepository.Edit(ordrsOrUserVM.orders);
           await _orderRepository.CommitAsync();
            TempData["success-notification"] = "Successfull update Orders";
            return RedirectToAction(nameof(Index));
        }

    }
}
