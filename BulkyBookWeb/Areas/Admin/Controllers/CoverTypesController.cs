#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BulkyBook.Models;
using BulkyBook.Data.Repository.Interface;

namespace BulkyBookWeb.Controllers
{
    [Area("Admin")]
    public class CoverTypesController : Controller
    {
        private readonly IUnitOfWork _unit;

        public CoverTypesController(IUnitOfWork unit)
        {
            _unit = unit;
        }

        // GET: CoverTypes
        public IActionResult Index()
        {
            return View(_unit.CoverType.GetAll());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,CreatedDateTime")] CoverType coverType)
        {
            if (ModelState.IsValid)
            {
                _unit.CoverType.Add(coverType);
                await _unit.Save();
                return RedirectToAction(nameof(Index));
            }
            return View(coverType);
        }

        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var coverType = _unit.CoverType.GetFirstOrDefault(r => r.Id == id);
            if (coverType == null)
            {
                return NotFound();
            }
            return View(coverType);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] CoverType coverType)
        {
            if (id != coverType.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _unit.CoverType.Update(coverType);
                    await _unit.Save();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CoverTypeExists(coverType.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(coverType);
        }

        // GET: CoverTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var coverType = _unit.CoverType
                .GetFirstOrDefault(r => r.Id == id);
            if (coverType == null)
            {
                return NotFound();
            }

            _unit.CoverType.Remove(coverType);
            await _unit.Save();

            return RedirectToAction(nameof(Index));
        }

        private bool CoverTypeExists(int id)
        {
            return _unit.CoverType.GetAll().Any(e => e.Id == id);
        }
    }
}
