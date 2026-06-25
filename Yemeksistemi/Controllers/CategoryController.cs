using Microsoft.AspNetCore.Mvc;
using Yemeksistemi.Models;
using System.Linq;

namespace Yemeksistemi.Controllers
{
    public class CategoryController : Controller
    {
        private readonly AppDbContext _context;

        public CategoryController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Rapor()
        {
            var data = _context.Categories
                .Select(x => new {
                    x.CategoryName
                }).ToList();

            ViewBag.Kategoriler = data;

            var totalCategory = _context.Categories.Count();
            ViewBag.TotalCategory = totalCategory;

            var longestCategory = _context.Categories
                .OrderByDescending(x => x.CategoryName.Length)
                .Select(x => x.CategoryName)
                .FirstOrDefault();

            ViewBag.LongestCategory = longestCategory;

            var aCount = _context.Categories
                .Count(x => x.CategoryName.StartsWith("A"));

            ViewBag.ACount = aCount;

            ViewBag.GroupKategori = (from r in _context.Recipes
                                     join c in _context.Categories
                                     on r.CategoryId equals c.CategoryId
                                     group r by c.CategoryName into g
                                     select new
                                     {
                                         Kategori = g.Key,
                                         Adet = g.Count()
                                     }).ToList();


            ViewBag.JoinData = _context.Recipes
                .Select(r => new {
                    Kategori = r.Category.CategoryName,
                    Yemek = r.RecipeName
                }).ToList();
            return View();
        }

        public IActionResult Index(string p)
        {
            var values = from x in _context.Categories select x;

            if (!string.IsNullOrEmpty(p))
            {
                values = values.Where(x => x.CategoryName.Contains(p));
            }

            return View(values.ToList());
        }

        public IActionResult AddCategory()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddCategory(Category category)
        {
            if (category == null)
            {
                return View();
            }

            if (string.IsNullOrEmpty(category.CategoryName))
            {
                return View();
            }

            _context.Categories.Add(category);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult DeleteCategory(int id)
        {
            var value = _context.Categories.Find(id);
            _context.Categories.Remove(value);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult EditCategory(int id)
        {
            var value = _context.Categories
                .FirstOrDefault(x => x.CategoryId == id);

            if (value == null)
                return NotFound();

            return View(value);
        }
        [HttpPost]
        public IActionResult EditCategory(Category category)
        {
            _context.Categories.Update(category);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

     
        }
    }
