using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Yemeksistemi.Models;

namespace Yemeksistemi.Controllers
{
    public class RecipeController : Controller
    {
        private readonly AppDbContext _context;

        public RecipeController(AppDbContext context)
        {
            _context = context;
        }

        // 📌 LISTELEME
        public IActionResult Index()
        {
            var values = _context.Recipes.ToList();
            return View(values);
        }

        // 📌 ADD (GET)
        public IActionResult AddRecipe()

        {
            ViewData["RecipeId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName");
            return View();
        }

        // 📌 ADD (POST)
        [HttpPost]
        public IActionResult AddRecipe(Recipe recipe)
        {
            _context.Recipes.Add(recipe);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // 📌 EDIT (GET)
        public IActionResult EditRecipe(int id)
        {
            var value = _context.Recipes.Find(id);

            ViewBag.Categories = _context.Categories.ToList();

            return View(value);
        }

        // 📌 EDIT (POST)
        [HttpPost]
        public IActionResult EditRecipe(Recipe recipe)
        {
            _context.Recipes.Update(recipe);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // 📌 DELETE
        public IActionResult DeleteRecipe(int id)
        {
            var value = _context.Recipes.Find(id);

            if (value != null)
            {
                _context.Recipes.Remove(value);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }
    }
}