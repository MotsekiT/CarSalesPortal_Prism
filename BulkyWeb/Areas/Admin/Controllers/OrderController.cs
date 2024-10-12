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

			OrderVM orderVM = new()
			{
				OrderHeader = _unitOfWork.OrderHeader.Get(u => u.Id== orderId, includeProperties:"ApplicationUser"),
				OrderDetail = _unitOfWork.OrderDetail.GetAll(u=> u.OrderHeaderId == orderId, includeProperties: "Car")

			};

            return View(orderVM);
        }




        #region API CALLS

        [HttpGet]
		public IActionResult GetAll()
		{
			List<OrderHeader> objOrderHeaders = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser").ToList();

		


            return Json(new { data = objOrderHeaders });
		}

		#endregion
	}
}



