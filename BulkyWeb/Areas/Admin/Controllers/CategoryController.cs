using CarSalesPortal.DataAccess.Data;
using CarSalesPortal.DataAccess.Repository;
using CarSalesPortal.DataAccess.Repository.IRepository;
using CarSalesPortal.Models;
using CarSalesPortal.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarSalesPortal.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public IActionResult Index()
        {
            // Retrieve all categories from the database using UnitOfWork pattern
            // and convert the result to a list.
            List<Category> objCatogoryList = _unitOfWork.Category.GetAll().ToList();

            // Pass the list of categories to the View to be displayed
            return View(objCatogoryList);
        }


        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Create(Category obj)
        {
            // Check if the Name property of the Category object matches
            // the ItemNumber property (converted to string)
            if (obj.Name == obj.ItemNumber.ToString())
            {
                ModelState.AddModelError("name", "The DisplayOrder cannot exactly match the Name.");
            }

            // Check if the model state is valid.
            if (ModelState.IsValid)
            {
                // If valid, add the category object to the database using UnitOfWork pattern
                _unitOfWork.Category.Add(obj);
                _unitOfWork.Save();// Save changes to the database
                TempData["success"] = "Category created successfully";

                // Redirect the user back to the Index action after successfully creating the category
                return RedirectToAction("Index");
            }

            return View();
        }

        public IActionResult Edit(int? id)
        {
            // Check if the id is null or 0, which indicates an invalid category id
            if (id == null || id == 0)
            {
                return NotFound();
            }
            // Retrieve the category from the database using the UnitOfWork pattern
            Category? category = _unitOfWork.Category.Get(u => u.Id == id);

            // If no category is found with the provided id, return a 404 Not Found response
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }
        [HttpPost]
        public IActionResult Edit(Category obj)
        {

            // Check if the model state is valid
            if (ModelState.IsValid)
            {
                //If the model is valid, update the category in the database using the UnitOfWork pattern
                _unitOfWork.Category.Update(obj);
                // Save changes to the database
                _unitOfWork.Save();
                TempData["success"] = "Category updated successfully";
                // Redirect the user to the Index action after successfully updating the category
                return RedirectToAction("Index");
            }

            return View();
        }


        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            Category? category = _unitOfWork.Category.Get(u => u.Id == id);


            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }


        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? id)
        {
            // Retrieve the category object from the database using the provided id
            Category? obj = _unitOfWork.Category.Get(u => u.Id == id);
            if (obj == null) { return NotFound(); }
            // Remove the category from the database using the UnitOfWork pattern
            _unitOfWork.Category.Remove(obj);
            // Save changes to the database
            _unitOfWork.Save();
            TempData["success"] = "Category deleted successfully";
            // Redirect the user to the Index action after successful deletion
            return RedirectToAction("Index");



        }
    }
}
