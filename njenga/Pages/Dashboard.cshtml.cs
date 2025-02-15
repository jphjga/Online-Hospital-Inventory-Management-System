using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Njenga.Data;
using Njenga.Models;
using System.Text.Json;

namespace Njenga.Pages
{
    public class DashboardModel : PageModel
    {
        private readonly DatabaseContext _context;

        public DashboardModel(DatabaseContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Product NewProduct { get; set; } = new Product();

        public IList<Product> Products { get; set; } = new List<Product>();
        public IList<Product> ExpiringProducts { get; set; } = new List<Product>();
        public IList<Institution> Institutions { get; set; } = new List<Institution>();
        public IList<Product> LowStockProducts { get; set; } = new List<Product>();
        public IList<PurchaseRecord> PurchasedProducts { get; set; } = new List<PurchaseRecord>(); // For Purchases tab
        public IList<PurchaseRecord> PendingPurchases { get; set; } = new List<PurchaseRecord>();   // For Add Purchases tab   
        public int ExpiredProductCount { get; set; } = 0;

        [BindProperty(SupportsGet = true)]
        public int? EditId { get; set; }

        [BindProperty]
        public DateTime? StartDate { get; set; } = DateTime.Now;

        [BindProperty]
        public DateTime? EndDate { get; set; } = DateTime.Now.AddMonths(3);

        public DateTime CurrentDateTime => DateTime.Now;
        public string CurrentUserName => HttpContext.Session.GetString("CurrentUserName") ?? "Unknown";

        // We include FoundProduct in case you still need it (for example, if you previously used a barcode lookup)
        public Product? FoundProduct { get; set; }
        //stock taking tab
        public IList<Product> StockTakingProducts { get; set; } = new List<Product>();

        public IActionResult OnGet(int? editId)
        {
            int? institutionId = HttpContext.Session.GetInt32("InstitutionId");
            if (institutionId == null)
            {
                return RedirectToPage("Index");
            }

            LoadData(institutionId.Value);

            // Load pending purchases from session for the Add Purchases tab.
            PendingPurchases = HttpContext.Session.GetObjectFromJson<List<PurchaseRecord>>("PendingPurchases")
                                ?? new List<PurchaseRecord>();

            // Load confirmed purchases from the database.
            PurchasedProducts = _context.PurchaseRecords
                .Where(p => p.InstitutionId == institutionId)
                .ToList();

            // If an edit is requested, load the product for editing.
            if (editId.HasValue)
            {
                NewProduct = _context.Products.FirstOrDefault(p => p.Product_id == editId) ?? new Product();
            }
            else
            {
                // For stock taking, we want the modal to be empty until a product is selected.
                NewProduct = new Product();
            }

            //stock taking tab.
            StockTakingProducts = HttpContext.Session.GetObjectFromJson<List<Product>>("StockTakingProducts")
                ?? new List<Product>();


            return Page();
        }


        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                int instId = HttpContext.Session.GetInt32("InstitutionId") ?? 0;
                LoadData(instId);
                return Page();
            }

            // Check if stock-taking data was submitted
            var stockTakingData = Request.Form.Where(k => k.Key.StartsWith("StockTakingProducts")).ToDictionary(k => k.Key, k => k.Value.ToString());

            if (stockTakingData.Count > 0)
            {
                // Stock Taking Mode
                foreach (var key in stockTakingData.Keys)
                {
                    if (key.Contains(".Product_id"))
                    {
                        int productId = int.Parse(stockTakingData[key]);
                        string quantityKey = key.Replace("Product_id", "Quantity");

                        if (stockTakingData.ContainsKey(quantityKey))
                        {
                            int newQuantity = int.Parse(stockTakingData[quantityKey]);

                            var existingProduct = _context.Products.FirstOrDefault(p => p.Product_id == productId);
                            if (existingProduct != null)
                            {
                                existingProduct.Quantity = newQuantity;  // Update Quantity
                            }
                        }
                    }
                }

                _context.SaveChanges();
                return RedirectToPage();
            }

            // Regular Product Update / Add Mode
            if (NewProduct.Product_id == 0)
            {
                _context.Products.Add(NewProduct);
            }
            else
            {
                var existingProduct = _context.Products.FirstOrDefault(p => p.Product_id == NewProduct.Product_id);
                if (existingProduct != null)
                {
                    existingProduct.Name = NewProduct.Name;
                    existingProduct.Description = NewProduct.Description;
                    existingProduct.Price = NewProduct.Price;
                    existingProduct.Quantity = NewProduct.Quantity;  // Updated Quantity
                    existingProduct.Expiry = NewProduct.Expiry;
                    existingProduct.Barcode = NewProduct.Barcode;
                    existingProduct.Quantity_alert = NewProduct.Quantity_alert;
                    existingProduct.Category = NewProduct.Category;
                    existingProduct.InstitutionId = NewProduct.InstitutionId;
                }
            }

            _context.SaveChanges();
            return RedirectToPage();
        }



        public IActionResult OnPostClearPendingPurchases()
        {
            HttpContext.Session.Remove("PendingPurchases");
            PendingPurchases = new List<PurchaseRecord>();
            return RedirectToPage();
        }

        public IActionResult OnPostSubmitPendingPurchases()
        {
            var pending = HttpContext.Session.GetObjectFromJson<List<PurchaseRecord>>("PendingPurchases")
                          ?? new List<PurchaseRecord>();
            foreach (var purchase in pending)
            {
                _context.PurchaseRecords.Add(purchase);
            }
            _context.SaveChanges();
            HttpContext.Session.Remove("PendingPurchases");
            return RedirectToPage();
        }

        public IActionResult OnPostFilterExpiry()
        {
            int? institutionId = HttpContext.Session.GetInt32("InstitutionId");
            if (institutionId == null)
                return RedirectToPage("Index");

            LoadData(institutionId.Value);
            return Page();
        }

        // Handler to add a pending purchase (existing functionality for the Add Purchases tab)
        public IActionResult OnPostAddPurchase(int productId)
        {
            var product = _context.Products.FirstOrDefault(p => p.Product_id == productId);
            if (product != null)
            {
                var purchaseRecord = new PurchaseRecord
                {
                    Name = product.Name,
                    Amount = null, // Optional
                    Quantity = product.Quantity,
                    Price = product.Price,
                    InstitutionId = product.InstitutionId
                };

                var pending = HttpContext.Session.GetObjectFromJson<List<PurchaseRecord>>("PendingPurchases")
                              ?? new List<PurchaseRecord>();
                pending.Add(purchaseRecord);
                HttpContext.Session.SetObjectAsJson("PendingPurchases", pending);
            }
            return RedirectToPage();
        }

        public IActionResult OnPostRemovePurchase(int id)
        {
            var purchase = _context.PurchaseRecords.FirstOrDefault(p => p.Id == id);
            if (purchase != null)
            {
                _context.PurchaseRecords.Remove(purchase);
                _context.SaveChanges();
            }
            return RedirectToPage();
        }

        // Load dashboard data.
        private void LoadData(int institutionId)
        {
            Products = _context.Products
                .Where(p => p.InstitutionId == institutionId)
                .Include(p => p.Institution)
                .ToList();

            Institutions = _context.Institutions.ToList();

            ExpiredProductCount = _context.Products
                .Where(p => p.InstitutionId == institutionId && p.Expiry != null && p.Expiry < DateTime.Now)
                .Count();

            ExpiringProducts = _context.Products
                .Where(p => p.InstitutionId == institutionId &&
                            p.Expiry != null &&
                            (p.Expiry < DateTime.Now ||
                             (StartDate == null || p.Expiry >= StartDate) &&
                             (EndDate == null || p.Expiry <= EndDate)))
                .OrderBy(p => p.Expiry)
                .ToList();

            LowStockProducts = _context.Products
                .Where(p => p.InstitutionId == institutionId &&
                            p.Quantity_alert.HasValue &&
                            p.Quantity <= p.Quantity_alert.Value)
                .OrderBy(p => p.Quantity)
                .ToList();
        }

        //stock taking
        public IActionResult OnPostAddToStockTaking(int productId)
        {
            var product = _context.Products.FirstOrDefault(p => p.Product_id == productId);
            if (product != null)
            {
                var stockTakingList = HttpContext.Session.GetObjectFromJson<List<Product>>("StockTakingProducts") ?? new List<Product>();
                stockTakingList.Add(product);
                HttpContext.Session.SetObjectAsJson("StockTakingProducts", stockTakingList);
            }
            return RedirectToPage();
        }

        //product detais model
        public IActionResult OnPostAddToStockTaking([FromBody] Product product)
        {
            if (product == null)
            {
                return new JsonResult(new { success = false, error = "Invalid product data." });
            }

            var stockTakingProducts = HttpContext.Session.GetObjectFromJson<List<Product>>("StockTakingProducts") ?? new List<Product>();

            // Avoid duplicates
            if (!stockTakingProducts.Any(p => p.Product_id == product.Product_id))
            {
                stockTakingProducts.Add(product);
                HttpContext.Session.SetObjectAsJson("StockTakingProducts", stockTakingProducts);
            }

            return new JsonResult(new { success = true });
        }



        public IActionResult OnPostStockTaking()
        {
            if (StockTakingProducts == null || StockTakingProducts.Count == 0)
            {
                ModelState.AddModelError("", "No products selected for stock taking.");
                return Page();
            }

            // Debug: Print received data
            foreach (var stockProduct in StockTakingProducts)
            {
                Console.WriteLine($"Received: Product ID = {stockProduct.Product_id}, Quantity = {stockProduct.Quantity}");
            }

            foreach (var stockProduct in StockTakingProducts)
            {
                var existingProduct = _context.Products.FirstOrDefault(p => p.Product_id == stockProduct.Product_id);
                if (existingProduct != null)
                {
                    existingProduct.Quantity = stockProduct.Quantity; // Update only quantity
                }
            }

            _context.SaveChanges();
            HttpContext.Session.Remove("StockTakingProducts"); // Clear session after saving
            return RedirectToPage();
        }



    }
}