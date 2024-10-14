using CarSalesPortal.DataAccess.Repository;
using CarSalesPortal.Models;
using CarSalesPortal.Models.ViewModels;
using CarSalesPortal.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CarSalesPortal.Areas.Customer.Controllers
{
    [Area("customer")]
    [Authorize]
    public class CartController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;
        [BindProperty]
        public ShoppingCartVM ShoppingCartVM { get; set; }

        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public IActionResult Summary()
        {
            // Retrieve the ClaimsIdentity of the current user
            var claimsIdentity = (ClaimsIdentity)User.Identity;

            // Get the User ID from the claims (NameIdentifier)
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;


            // Initialize a new instance of ShoppingCartVM (ViewModel) to hold shopping cart and order information
            ShoppingCartVM = new()
            {
                ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId,
                includeProperties: "Car"),
                OrderHeader = new()
            };

            // Retrieve the ApplicationUser associated with the userId
            ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);

            // Populate the OrderHeader fields with the user's information
            ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.ApplicationUser.Name;
            ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
            ShoppingCartVM.OrderHeader.StreetAddress = ShoppingCartVM.OrderHeader.ApplicationUser.StreetName;
            ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.City;
            ShoppingCartVM.OrderHeader.State = ShoppingCartVM.OrderHeader.ApplicationUser.State;
            ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.ApplicationUser.PostalCode;
            ShoppingCartVM.OrderHeader.Email = ShoppingCartVM.OrderHeader.ApplicationUser.Email;
            


           

            return View(ShoppingCartVM);



        }

        [HttpPost]
        [ActionName("Summary")]
        public IActionResult SummaryPOST()
        {
            // Retrieve the ClaimsIdentity of the current user
            var claimsIdentity = (ClaimsIdentity)User.Identity;

            // Get the User ID from the claims (NameIdentifier)
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            // Retrieve all shopping cart items associated with the userId
            ShoppingCartVM.ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId,
                includeProperties: "Car");


            //ShoppingCartVM.OrderHeader.OrderDate = System.DateTime.Now;
            ShoppingCartVM.OrderHeader.ApplicationUserId = userId;

            
            // Retrieve the ApplicationUser associated with the userId
            ApplicationUser applicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);

            // Add the OrderHeader to the database
            _unitOfWork.OrderHeader.Add(ShoppingCartVM.OrderHeader);
            _unitOfWork.Save();

            // Iterate through each item in the shopping cart to create order details
            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                // Create a new OrderDetail object for each item in the shopping cart
                OrderDetail orderDetail = new()
                {
                    CarId = cart.CarId,
                    OrderHeaderId = ShoppingCartVM.OrderHeader.Id,
                    Price = cart.Car.Price
                    
                };
                // Add the OrderDetail to the database
                _unitOfWork.OrderDetail.Add(orderDetail);
                _unitOfWork.Save();
            }

            // Redirect to the OrderConfirmation action, passing the OrderHeader Id
            return RedirectToAction(nameof(OrderConfirmation), new {id=ShoppingCartVM.OrderHeader.Id});



        }

        public IActionResult OrderConfirmation(int id)
        {
            // Retrieve the OrderHeader from the database using the provided id, including related ApplicationUser data
            OrderHeader orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == id, includeProperties: "ApplicationUser");

            // Retrieve all shopping cart items associated with the userId from the order header
            List<ShoppingCart> shoppingCartList = _unitOfWork.ShoppingCart
                .GetAll(u => u.ApplicationUserId == orderHeader.ApplicationUserId).ToList();
            // Remove all shopping cart items for the user after the order confirmation
            _unitOfWork.ShoppingCart.RemoveRange(shoppingCartList);
            // Save changes to the database to persist the removal of the shopping cart items
            _unitOfWork.Save();

            // Return the view for the order confirmation, passing the order id
            return View(id);
        }




        public IActionResult Index()
        {
            // Retrieve the ClaimsIdentity of the current user
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            // Get the User ID from the claims (NameIdentifier)
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;


            // Initialize a new instance of ShoppingCartVM (ViewModel) to hold shopping cart and order information
            ShoppingCartVM = new()
            {
                ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId,
                includeProperties: "Car"),
                OrderHeader = new()
            };


            // Pass the populated ShoppingCartVM to the View for rendering
            return View(ShoppingCartVM);
        }

       

        public IActionResult Remove(int cartId)
        {
            // Retrieve the shopping cart item from the database using the provided cartId
            var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId, tracked:true);

            // Update the session variable that tracks the number of items in the shopping cart
            // Get the count of all shopping cart items for the current user and subtract 1
            HttpContext.Session.SetInt32(SD.SessionCart,
                 _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == cartFromDb.ApplicationUserId).Count() - 1);

            // Remove the retrieved shopping cart item from the database
            _unitOfWork.ShoppingCart.Remove(cartFromDb);

            // Save changes to the database
            _unitOfWork.Save();

            // Redirect the user back to the Index action after removal
            return RedirectToAction(nameof(Index));
        }

    }
}
