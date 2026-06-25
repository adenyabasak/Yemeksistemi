using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Yemeksistemi.Controllers
{
    public class AdminController : Controller
    {
        private readonly AppDbContext dbcontext;

        public AdminController(AppDbContext dbcontext)
        {
            this.dbcontext = dbcontext;
        }

        public IActionResult Index()
        {
            var result = dbcontext.Categories.ToList();
            return View(result);
        }
    }
}
