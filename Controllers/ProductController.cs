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

        [HttpGet]
        public async Task<IActionResult> EditProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProduct(Product product)
        {
            if (!ModelState.IsValid)
            {
                return View(product);
            }
            var dbProduct = await _context.Products.FindAsync(product.ProductId);
            if (dbProduct == null)
            {
                return NotFound();
            }
            dbProduct.ProductName = product.ProductName;
            dbProduct.BasicRate = product.BasicRate;
            dbProduct.MRP = product.MRP;
            dbProduct.SellingPrice = product.SellingPrice;
            dbProduct.CGSTPercentage = product.CGSTPercentage;
            dbProduct.SGSTPercentage = product.SGSTPercentage;
            //dbProduct.UnitsPerCrate = product.UnitsPerCrate;
            dbProduct.IsGSTAplicable = product.IsGSTAplicable;
            dbProduct.BuyingCost = product.IsGSTAplicable
                ? product.BasicRate + (product.BasicRate * (product.CGSTPercentage / 100)) + (product.BasicRate * (product.SGSTPercentage / 100))
                : product.BasicRate;
            _context.Update(dbProduct);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
