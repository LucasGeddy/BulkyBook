using BulkyBook.Data;
using BulkyBook.Data.Repository.Interface;
using BulkyBook.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBookWeb.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unit;

        public CategoryController(IUnitOfWork unit)
        {
            _unit = unit;
        }
        public IActionResult Index()
        {
            IEnumerable<Category> objCategoryList = _unit.Category.GetAll();

            return View(objCategoryList);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category category)
        {
            if (category.Name == category.DisplayOrder.ToString())
            {
                ModelState.AddModelError("Name", "The Display Order cannot match the Category's Name");
            }

            if (ModelState.IsValid)
            {
                _unit.Category.Add(category);
                _unit.Save();

                TempData["success"] = "Category created successfully!";

                return RedirectToAction("Index");
            }
            return View(category);
        }

        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var categoryFromDb = _unit.Category.GetFirstOrDefault(r=> r.Id == id);

            if (categoryFromDb == null)
            {
                return NotFound();
            }
            
            return View(categoryFromDb);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category category)
        {
            if (category.Name == category.DisplayOrder.ToString())
            {
                ModelState.AddModelError("Name", "The Display Order cannot match the Category's Name");
            }

            if (ModelState.IsValid)
            {
                _unit.Category.Update(category);
                _unit.Save();
                TempData["success"] = "Category updated successfully!";

                return RedirectToAction("Index");
            }
            return View(category);
        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var category = _unit.Category.GetFirstOrDefault(r => r.Id == id);

            if (category == null)
            {
                return NotFound();
            }

            _unit.Category.Remove(category);
            _unit.Save();
            TempData["success"] = "Category deleted successfully!";

            return RedirectToAction("Index");
        }
    }
}
