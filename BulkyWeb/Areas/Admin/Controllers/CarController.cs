using CarSalesPortal.DataAccess.Data;
using CarSalesPortal.DataAccess.Repository;
using CarSalesPortal.DataAccess.Repository.IRepository;
using CarSalesPortal.Models;
using CarSalesPortal.Models.ViewModels;
using CarSalesPortal.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace CarSalesPortal.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CarController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CarController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            // Retrieve all Car records from the database, including their associated Category data.
            // The _unitOfWork is handling the database operations, and GetAll is likely a method that returns all the Cars.
            List<Car> objCarList = _unitOfWork.Car.GetAll(includeProperties:"Category").ToList();


            // Pass the list of Car objects (objCarList) to the view for rendering.
            return View(objCarList);
        }


        public IActionResult Upsert(int? id)
        {
            // Create a new instance of CarVM (ViewModel) to pass to the view.
            // Initialize CategoryList by fetching all categories from the database
            // and converting them to SelectListItem objects for dropdowns.

            CarVM carVm = new()
            {
                CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                Car = new Car()
            };

            // Check if the id is null or 0, meaning it's a request to create a new car.
            if (id == null || id == 0)
            {
                // If no ID is provided, return the view with an empty Car object for a new car creation form.
                return View(carVm);
            }
            else
            {
                // If an ID is provided, fetch the Car entity with the corresponding ID from the database.
                carVm.Car = _unitOfWork.Car.Get(u=>u.Id==id);
                // Return the view with the carVm, populated with the car details for editing.
                return View(carVm);
            }
        }
        [HttpPost]
        public IActionResult Upsert(CarVM carVM, IFormFile? file)
        {
            // Check if the model state is valid, meaning that the validation rules defined for CarVM have been satisfied.
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (file != null) 
                {
                    // Get the root path of the web hosting environment (wwwroot) where images will be saved.
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(wwwRootPath, @"images\products");


                    // If there is already an image associated with the car, delete the old image.
                    if (carVM.Car.ImageUrl != null) 
                    {
                        //delete the old image

                        var oldImage = 
                            Path.Combine(wwwRootPath, carVM.Car.ImageUrl.Trim('/'));

                        // Check if the old image file exists, and if so, delete it.
                        if (System.IO.File.Exists(oldImage))
                        {
                            System.IO.File.Delete(oldImage);
                        }

                    }

                    // Save the new uploaded image file to the designated product path.
                    using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    // Set the ImageUrl property of the Car object to the path of the newly uploaded image.
                    carVM.Car.ImageUrl = @"/images/products/" + fileName;
                }

                // If Car Id is 0, it means a new car is being created, so add the new Car record to the database.
                if (carVM.Car.Id == 0) 
                {
                    _unitOfWork.Car.Add(carVM.Car);
                }
                else
                {
                    // Otherwise, it's an update of an existing Car record.
                    _unitOfWork.Car.Update(carVM.Car);
                }

                // Save the changes to the database.
                _unitOfWork.Save();
                TempData["success"] = "Advert created successfully";
                // Redirect to the Index action (likely showing a list of all cars).
                return RedirectToAction("Index");
            }
            else
            {
                // If the model state is not valid (e.g validation errors), reload the CategoryList for the dropdown in the form.
                carVM.CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,// Display the category name in the dropdown.
                    Value = u.Id.ToString() // Set the category ID as the value for the dropdown.
                });
                // Return the view with the carVM, showing validation errors if any.  
                return View(carVM);
            }

            
        }

        
        

        #region API CALLS

        //For API-Table
        [HttpGet]
        public IActionResult GetAll()
        {
            //Fetch all Car records from the database, including their associated Category data.
            // The 'includeProperties: "Category"' indicates that the related Category information will be eagerly loaded.
            List<Car> objCarList = _unitOfWork.Car.GetAll(includeProperties: "Category").ToList();


            // Return the list of cars (objCarList) as a JSON object. 
            // The data is encapsulated in a JSON object with a property 'data' containing the car list.
            return Json(new {data = objCarList });
        }



        [HttpDelete]
        public IActionResult Delete(int? id)
          {
            // Fetch the car with the provided id from the database.
            var carToDelete = _unitOfWork.Car.Get(u => u.Id == id);

            // Check if the car with the given id exists. If not, return a JSON response with an error message.
            if (carToDelete == null)
            {
                return Json(new {success = false, message = "Error while deleting"});
            }

            var oldImage =
                            Path.Combine(_webHostEnvironment.WebRootPath,
                            carToDelete.ImageUrl.Trim('/'));

            // Check if the image file exists in the system, and if it does, delete it.
            if (System.IO.File.Exists(oldImage))
            {
                System.IO.File.Delete(oldImage);
            }
            // Remove the car record from the database.
            _unitOfWork.Car.Remove(carToDelete);
            // Save the changes to the database to complete the deletion.
            _unitOfWork.Save();

            // Return a success message as a JSON response to confirm the deletion was successful.
            return Json(new { success = false, message = "Deleted Successful" });
        }
        #endregion
    }
}
