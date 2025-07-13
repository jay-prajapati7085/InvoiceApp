using InvoiceApp.Models;
using InvoiceApp.NewFolder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rotativa.AspNetCore;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InvoiceApp.Controllers
{
    public class OrderController : Controller
    {

        private readonly ApplicationDbContext _context;

        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Create(DateTime? date = null)
        {
            var products = await _context.Products.ToListAsync();
            var model = new OrderCreateViewModel
            {
                Products = products,
                InvoiceDate = date ?? DateTime.Today
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OrderCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Products = await _context.Products.ToListAsync();
                return View(model);
            }
            
            var orderItems = new List<OrderItem>();

            //var invoice = _context.Invoices
            //    .Where(i => i.InvoiceDate == model.InvoiceDate).FirstOrDefault();
            var invoice = await _context.Invoices
                .Include(i => i.OrderItems)
                .FirstOrDefaultAsync(i => i.InvoiceDate == model.InvoiceDate);

            var existingOrdersByDate = await _context.OrderItems
                .Include(p => p.Product)
                .Where(p => p.OrderDate == model.InvoiceDate).ToListAsync();

            foreach (var item in model.OrderItems)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product == null)
                {
                    ModelState.AddModelError("", $"Product with ID {item.ProductId} not found.");
                    model.Products = await _context.Products.ToListAsync();
                    return View(model);
                }


                if (existingOrdersByDate != null 
                    && existingOrdersByDate.Any(p => p.Product.ProductId == item.ProductId))
                {
                    var product1 = await _context.Products.FindAsync(item.ProductId);
                    continue;
                }
                    

                orderItems.Add(new OrderItem
                {
                    Product = product,
                    OrderDate = model.InvoiceDate,
                    Quntity = item.Quntity,
                    TotalSellingCost = product.SellingPrice * item.Quntity
                });
            }

            if(invoice != null)
            {
                invoice.OrderItems.AddRange(orderItems);
                invoice.TotalAmount = invoice.OrderItems.Sum(oi => oi.TotalSellingCost);
                _context.Update(invoice);
            }
            else
            {
                invoice = new Invoice
                {
                    InvoiceDate = model.InvoiceDate,
                    OrderItems = orderItems,
                    TotalAmount = model.TotalAmount
                };
                
                _context.Invoices.Add(invoice);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Details", new { id = invoice.InvoiceId });
        }

        [HttpGet]
        public async Task<IActionResult> OrderList(string search, DateTime? fromDate, DateTime? toDate, int page = 1, int pageSize = 10)
        {
            var query = _context.OrderItems.Include(o => o.Product).AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(o => o.Product.ProductName.Contains(search));
            }
            if (fromDate.HasValue)
            {
                query = query.Where(o => o.OrderDate >= fromDate.Value);
            }
            if (toDate.HasValue)
            {
                query = query.Where(o => o.OrderDate <= toDate.Value);
            }

            var totalCount = await query.CountAsync();
            var orderList = await query
                .OrderByDescending(o => o.OrderDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(o => new OrderLIstViewModel
                {
                    OrderId = o.OrderId,
                    OrderDate = o.OrderDate,
                    ProductId = o.Product.ProductId,
                    ProductName = o.Product.ProductName,
                    BasicRate = o.Product.BasicRate,
                    BuyingCost = o.Product.BuyingCost,
                    SellingPrice = o.Product.SellingPrice,
                    CrateCount = o.CrateCount,
                    Quntity = o.Quntity,
                    UnitsPerCrate = o.UnitsPerCrate
                })
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = totalCount;
            ViewBag.Search = search;
            ViewBag.FromDate = fromDate?.ToString("yyyy-MM-dd");
            ViewBag.ToDate = toDate?.ToString("yyyy-MM-dd");

            return View(orderList);
        }

        public async Task<IActionResult> Details(int id)
        {
            var invoice = await _context.Invoices
                .Include(i => i.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(i => i.InvoiceId == id);

            if (invoice == null)
            {
                return NotFound();
            }

            var viewModel = new InvoiceViewModel
            {
                Invoice = invoice,
                AmountInWords = NumberToWordsConverter.Convert(invoice.TotalAmount)
            };

            return View(viewModel);
        }

        public async Task<IActionResult> BulkExport(DateTime fromDate, DateTime toDate)
        {
            var invoices = await _context.Invoices
                .Include(i => i.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Where(i => i.InvoiceDate >= fromDate && i.InvoiceDate <= toDate)
                .ToListAsync();

            if (!invoices.Any())
            {
                return NotFound("No invoices found in the specified date range.");
            }

            using var memoryStream = new MemoryStream();
            using (var archive = new System.IO.Compression.ZipArchive(memoryStream, System.IO.Compression.ZipArchiveMode.Create, true))
            {
                foreach (var invoice in invoices)
                {
                    var viewModel = new InvoiceViewModel
                    {
                        Invoice = invoice,
                        AmountInWords = NumberToWordsConverter.Convert(invoice.TotalAmount)
                    };

                    var invoicePdfView = new ViewAsPdf("Invoice", viewModel)
                    {
                        PageSize = Rotativa.AspNetCore.Options.Size.A4,
                        PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
                        FileName = $"Invoice_{invoice.InvoiceId}_{invoice.InvoiceDate:yyyyMMdd}.pdf"
                    };

                    byte[] pdfBytes = await invoicePdfView.BuildFile(ControllerContext);

                    var entry = archive.CreateEntry($"Invoice_{invoice.InvoiceId}_{invoice.InvoiceDate:yyyyMMdd}.pdf");
                    using var entryStream = entry.Open();
                    await entryStream.WriteAsync(pdfBytes, 0, pdfBytes.Length);
                }
            }

            memoryStream.Position = 0;
            string zipFileName = $"Invoices_{fromDate:yyyyMMdd}_{toDate:yyyyMMdd}.zip";
            return File(memoryStream.ToArray(), "application/zip", zipFileName);
        }

        public async Task<IActionResult> ExportPdf(int id)
        {
            var invoice = await _context.Invoices
                .Include(i => i.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(i => i.InvoiceId == id);

            if (invoice == null)
            {
                return NotFound();
            }

            var viewModel = new InvoiceViewModel
            {
                Invoice = invoice,
                AmountInWords = NumberToWordsConverter.Convert(invoice.TotalAmount)
            };

            ///

            var invoicePdfView = new ViewAsPdf("Invoice", viewModel)
            {
                PageSize = Rotativa.AspNetCore.Options.Size.A4,
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
                FileName = $"Invoice_{invoice.InvoiceId}_{invoice.InvoiceDate:yyyyMMdd}.pdf"
            };


            // Get the PDF byte array
            byte[] pdfBytes = await invoicePdfView.BuildFile(ControllerContext);

            // Build folder path based on month
            string folderName = invoice.InvoiceDate.ToString("yyyy-MM");
            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Invoices", folderName);

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            // Save PDF to file
            string fileName = $"invoice-{invoice.InvoiceId}.pdf";
            string fullPath = Path.Combine(folderPath, fileName);
            await System.IO.File.WriteAllBytesAsync(fullPath, pdfBytes);

            //// Optional: store the path in DB
            //invoice.PdfPath = $"/Invoices/{folderName}/{fileName}";
            //await _context.SaveChangesAsync();

            // Return file to download or view
            return File(pdfBytes, "application/pdf", fileName);
            //

            //return new ViewAsPdf("Invoice", viewModel)
            //{
            //    FileName = $"Invoice_{invoice.InvoiceId}_{invoice.InvoiceDate:yyyyMMdd}.pdf",
            //};

            
        }

        [HttpGet]
        public async Task<IActionResult> PreviewInvoice(int id)
        {
            var invoice = await _context.Invoices
                .Include(i => i.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(i => i.InvoiceId == id);

            if (invoice == null)
            {
                return NotFound();
            }

            var viewModel = new InvoiceViewModel
            {
                Invoice = invoice,
                AmountInWords = NumberToWordsConverter.Convert(invoice.TotalAmount)
            };

            return View("Invoice", viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> EditOrder(int id)
        {
            var orderItem = await _context.OrderItems
                .Include(o => o.Product)
                .FirstOrDefaultAsync(o => o.OrderId == id);
            if (orderItem == null)
            {
                return NotFound();
            }
            ViewBag.Products = new SelectList(await _context.Products.ToListAsync(), "ProductId", "ProductName", orderItem.Product?.ProductId);
            return View(orderItem);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditOrder(OrderItem model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Products = new SelectList(await _context.Products.ToListAsync(), "ProductId", "ProductName", model.Product?.ProductId);
                return View(model);
            }
            var orderItem = await _context.OrderItems.Include(o => o.Product).FirstOrDefaultAsync(o => o.OrderId == model.OrderId);
            if (orderItem == null)
            {
                return NotFound();
            }
            var product = await _context.Products.FindAsync(model.Product.ProductId);
            if (product == null)
            {
                ModelState.AddModelError("Product.ProductId", "Product not found.");
                ViewBag.Products = new SelectList(await _context.Products.ToListAsync(), "ProductId", "ProductName", model.Product?.ProductId);
                return View(model);
            }
            orderItem.OrderDate = model.OrderDate;
            orderItem.Product = product;
            orderItem.Quntity = model.Quntity;
            orderItem.CrateCount = model.CrateCount;
            orderItem.UnitsPerCrate = model.UnitsPerCrate;
            orderItem.TotalSellingCost = product.SellingPrice * model.Quntity;
            _context.Update(orderItem);
            await _context.SaveChangesAsync();
            return RedirectToAction("OrderList");
        }
    }
}
