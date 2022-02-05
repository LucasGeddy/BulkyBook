#nullable disable
using Microsoft.AspNetCore.Mvc;
using BulkyBook.Data.Repository.Interface;
using Microsoft.AspNetCore.Mvc.Rendering;
using BulkyBook.Models.ViewModels;

namespace BulkyBookWeb.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unit;
        private readonly IWebHostEnvironment _webHost;

        public ProductController(IUnitOfWork unit, IWebHostEnvironment webHost)
        {
            _unit = unit;
            _webHost = webHost;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upsert(int? id)
        {
            ProductVM productVM = InitViewModel();

            if (id != null && id != 0)
            {
                productVM.product = _unit.Product.GetFirstOrDefault(r => r.Id == id);
                if (productVM.product == null)
                {
                    return NotFound();
                }
            }

            return View(productVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(ProductVM productVM, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHost.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(wwwRootPath, @"images\products");
                    var extension = Path.GetExtension(file.FileName);

                    if (productVM.product.ImageUrl != null)
                    {
                        var oldImagePath = Path.Combine(wwwRootPath, productVM.product.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    using (var fileStreams = new FileStream(Path.Combine(uploads, fileName+extension), FileMode.Create))
                    {
                        file.CopyTo(fileStreams);
                    }
                    productVM.product.ImageUrl = @"\images\products\" + fileName + extension;
                }

                if (productVM.product.Id != 0)
                {
                    _unit.Product.Update(productVM.product);
                    TempData["success"] = "Product updated succesfully!";
                }
                else
                {
                    _unit.Product.Add(productVM.product);
                    TempData["success"] = "Product created succesfully!";
                }
                await _unit.Save();
                
                return RedirectToAction(nameof(Index));
            }

            var returnVM = InitViewModel();
            returnVM.product = productVM.product;
            TempData["error"] = "ERROR: Product was not added/updated!";
            return View(returnVM);
        }
        #region API Calls
        [HttpGet]
        public IActionResult GetAll()
        {
            var productList = _unit.Product.GetAll(includeProperties: "Category");
            return Json(new { data = productList });
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return Json(new { success = false, message = "Error deleting product. ID wasn't passed to the API" });
            }

            var product = _unit.Product.GetFirstOrDefault(r => r.Id == id);
            if (product == null)
            {
                return Json(new { success = false, message = "Error deleting product. NotFound" });
            }

            var imagePath = Path.Combine(_webHost.WebRootPath, product.ImageUrl.TrimStart('\\'));
            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }

            _unit.Product.Remove(product);
            await _unit.Save();

            return Json(new { success = true, message = "Product deleted successfully!" });
        }
        #endregion
        #region Helper Functions
        private bool ProductExists(int id)
        {
            return _unit.Product.GetAll().Any(e => e.Id == id);
        }

        private ProductVM InitViewModel()
        {
            return new ProductVM()
            {
                product = new(),
                categoryList = _unit.Category.GetAll().Select(r => new SelectListItem
                {
                    Text = r.Name,
                    Value = r.Id.ToString()
                }),
                coverTypeList = _unit.CoverType.GetAll().Select(r => new SelectListItem
                {
                    Text = r.Name,
                    Value = r.Id.ToString()
                }),
            };
        }
        #endregion
    }
}
