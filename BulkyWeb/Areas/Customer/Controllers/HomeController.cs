using CarSalesPortal.DataAccess.Repository;
using CarSalesPortal.Models;
using CarSalesPortal.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace CarSalesPortal.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index(string searchString)
        {  
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if(userId != null)
            {
                HttpContext.Session.SetInt32(SD.SessionCart,
                    _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId.Value).Count());

            }


            IEnumerable<Car> carList = _unitOfWork.Car.GetAll(includeProperties: "Category");

            if(!String.IsNullOrEmpty(searchString))
            {
                carList = carList.Where(n => n.Make.ToLower().Contains(searchString) || n.Price.ToString().Contains(searchString)).ToList();
            } 


            return View(carList);
        }

        public IActionResult Details(int productId)
        {

            ShoppingCart cart = new()
            {
                Car = _unitOfWork.Car.Get(u => u.Id == productId, includeProperties: "Category"),
                Count = 1,
                CarId = productId

            };

          
            return View(cart);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            shoppingCart.ApplicationUserId = userId;

            ShoppingCart cartFromDb = _unitOfWork.ShoppingCart.Get(u=>u.ApplicationUserId == userId && 
            u.CarId == shoppingCart.CarId);

            if (cartFromDb != null) 
            { //shopping cart already exists

                
                _unitOfWork.ShoppingCart.Update(cartFromDb);
                _unitOfWork.Save();
            }
            else
            { // add cart record
                _unitOfWork.ShoppingCart.Add(shoppingCart);
                _unitOfWork.Save();
                HttpContext.Session.SetInt32(SD.SessionCart, 
                    _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId).Count());

            }
            TempData["success"] = "Cart updated successfully";
          

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
