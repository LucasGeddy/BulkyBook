#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            return View(_unit.Product.GetAll());
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

                    using (var fileStreams = new FileStream(Path.Combine(uploads, fileName+extension), FileMode.Create))
                    {
                        file.CopyTo(fileStreams);
                    }
                    productVM.product.ImageUrl = @"\images\products\" + fileName + extension;
                }

                _unit.Product.Add(productVM.product);
                await _unit.Save();
                TempData["success"] = "Product created succesfully!";
                return RedirectToAction(nameof(Index));
            }

            var returnVM = InitViewModel();
            returnVM.product = productVM.product;
            return View(returnVM);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = _unit.Product.GetFirstOrDefault(r => r.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            _unit.Product.Remove(product);
            await _unit.Save();

            return RedirectToAction(nameof(Index));
        }

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
    }
}
