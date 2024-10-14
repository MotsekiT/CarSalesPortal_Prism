using CarSalesPortal.DataAccess.Repository;
using CarSalesPortal.Models;
using CarSalesPortal.Models.ViewModels;
using CarSalesPortal.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Core.Types;
using System.Security.Claims;

namespace CarSalesPortal.Areas.Admin.Controllers
{
	[Area("admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class OrderController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;


        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
		{
			return View();
		}


        public IActionResult Details(int orderId)
        {
            // Create a new instance of the OrderVM (ViewModel) to hold order details and header information
            OrderVM orderVM = new()
			{
                // Retrieve the OrderHeader from the database using the orderId
                // Include the related ApplicationUser property 
                OrderHeader = _unitOfWork.OrderHeader.Get(u => u.Id== orderId, includeProperties:"ApplicationUser"),
                // Retrieve all OrderDetails related to the orderId
                // Include the related Car property 
                OrderDetail = _unitOfWork.OrderDetail.GetAll(u=> u.OrderHeaderId == orderId, includeProperties: "Car")

			};
            // Pass the orderVM object to the view for rendering
            return View(orderVM);
        }




        #region API CALLS

        [HttpGet]
		public IActionResult GetAll()
		{
            // Retrieve all OrderHeader records from the database and include related ApplicationUser data 
            List<OrderHeader> objOrderHeaders = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser").ToList();



            // Return the list of OrderHeader objects as a JSON response, useful for APIs
            return Json(new { data = objOrderHeaders });
		}

		#endregion
	}
}



