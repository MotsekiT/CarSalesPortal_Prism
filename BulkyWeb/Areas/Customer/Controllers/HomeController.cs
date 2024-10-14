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
            // Retrieve the ClaimsIdentity of the current user
            var claimsIdentity = (ClaimsIdentity)User.Identity;

            // Get the User ID from the claims (NameIdentifier)
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);


            // Check if the userId is not null
            if (userId != null)
            {
                // Set the session variable that tracks the number of items in the shopping cart for the current user
                HttpContext.Session.SetInt32(SD.SessionCart,
                    _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId.Value).Count());

            }

            // Retrieve all cars from the database, including their associated Category data
            IEnumerable<Car> carList = _unitOfWork.Car.GetAll(includeProperties: "Category");

            // If a search string is provided, filter the car list based on the make or price
            if (!String.IsNullOrEmpty(searchString))
            {
                carList = carList.Where(n => n.Make.ToLower().Contains(searchString) || n.Price.ToString().Contains(searchString)).ToList();
            }

            // Return the filtered (or unfiltered) list of cars to the View for rendering
            return View(carList);
        }

        public IActionResult Details(int productId)
        {
            // Create a new instance of ShoppingCart to hold the details of the selected car
            ShoppingCart cart = new()
            {
                // Retrieve the Car details from the database using the provided productId,
                // including related Category data for additional context
                Car = _unitOfWork.Car.Get(u => u.Id == productId, includeProperties: "Category"),
                Count = 1,
                CarId = productId

            };

            // Return the ShoppingCart object to the View for rendering
            return View(cart);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            // Retrieve the ClaimsIdentity of the current user
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            // Get the User ID from the claims (NameIdentifier)
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            // Assign the retrieved User ID to the shopping cart object
            shoppingCart.ApplicationUserId = userId;

            // Check if a shopping cart entry already exists for this user and car
            ShoppingCart cartFromDb = _unitOfWork.ShoppingCart.Get(u=>u.ApplicationUserId == userId && 
            u.CarId == shoppingCart.CarId);

            if (cartFromDb != null)
            { // Shopping cart already exists for this user and car, so update it


                _unitOfWork.ShoppingCart.Update(cartFromDb);
                _unitOfWork.Save();
            }
            else
            { // No existing cart record found, so add the new shopping cart item
                _unitOfWork.ShoppingCart.Add(shoppingCart);
                _unitOfWork.Save();
                // Update the session variable that tracks the number of items in the shopping cart
                HttpContext.Session.SetInt32(SD.SessionCart, 
                    _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId).Count());

            }
            TempData["success"] = "Cart updated successfully";

            // Redirect the user back to the Index action
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
