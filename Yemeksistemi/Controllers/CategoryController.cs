using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Linq;
using Yemeksistemi.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Yemeksistemi.Controllers
{
    public class CategoryController : Controller
    {
        private readonly AppDbContext _context;

        public CategoryController(AppDbContext context)
        {
            _context = context;
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
            if (string.IsNullOrEmpty(category.CategoryName))
            {
                return View(category);
            }

            _context.Categories.Add(category);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult DeleteCategory(int id)
        {
            var value = _context.Categories.Find(id);

            if (value == null)
                return NotFound();

            _context.Categories.Remove(value);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult EditCategory(int id)
        {
            var value = _context.Categories.Find(id);

            if (value == null)
                return NotFound();

            return View(value);
        }

        [HttpPost]
        public IActionResult EditCategory(Category category)
        {
            var value = _context.Categories.Find(category.CategoryId);

            if (value == null)
                return NotFound();

            value.CategoryName = category.CategoryName;

            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Rapor()
        {
            var data = _context.Categories
                .Select(x => new
                {
                    x.CategoryName
                }).ToList();

            ViewBag.Kategoriler = data;

            ViewBag.TotalCategory = _context.Categories.Count();

            ViewBag.LongestCategory = _context.Categories
                .OrderByDescending(x => x.CategoryName.Length)
                .Select(x => x.CategoryName)
                .FirstOrDefault();

            ViewBag.ACount = _context.Categories
                .Count(x => x.CategoryName.StartsWith("A"));

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
                .Select(r => new
                {
                    Kategori = r.Category.CategoryName,
                    Yemek = r.RecipeName
                }).ToList();

            return View();
        }
    


    public IActionResult ExportExcel()
        {
            var categories = _context.Categories.ToList();

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Kategoriler");

            worksheet.Cell(1, 1).Value = "ID";
            worksheet.Cell(1, 2).Value = "Kategori Adı";

            int row = 2;

            foreach (var item in categories)
            {
                worksheet.Cell(row, 1).Value = item.CategoryId;
                worksheet.Cell(row, 2).Value = item.CategoryName;
                row++;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);

            return File(
                stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "KategoriListesi.xlsx"
            );
        }

        public IActionResult ExportPdf()
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var categories = _context.Categories.ToList();

            var pdf = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    page.Header()
                        .Text("Kategori Listesi Raporu")
                        .SemiBold()
                        .FontSize(20);

                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(80);
                            columns.RelativeColumn();
                        });

                        table.Header(header =>
                        {
                            header.Cell().Text("ID").SemiBold();
                            header.Cell().Text("Kategori Adı").SemiBold();
                        });

                        foreach (var item in categories)
                        {
                            table.Cell().Text(item.CategoryId.ToString());
                            table.Cell().Text(item.CategoryName);
                        }
                    });

                    page.Footer()
                        .AlignCenter()
                        .Text("Yemek Sistemi");
                });
            }).GeneratePdf();

            return File(pdf, "application/pdf", "KategoriListesi.pdf");
        }
    }
}

