using InvoiceApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InvoiceApp.Controllers
{
    public class ProductController : Controller
    {

        private readonly ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            //var products = await _context.Products.ToListAsync();
            return View("create");
        }

        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            if (ModelState.IsValid)
            {
                if (product.IsGSTAplicable)
                {
                    decimal CGSTAmout = product.BasicRate * (product.CGSTPercentage/100);
                    decimal SGSTAmout = product.BasicRate * (product.SGSTPercentage / 100);
                    product.BuyingCost = product.BasicRate + CGSTAmout + SGSTAmout;
                }
                else
                {
                    product.BuyingCost = product.BasicRate;
                }
                    _context.Products.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }
    }
}
