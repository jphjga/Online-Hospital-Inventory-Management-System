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
        public string Username { get; private set; } = "Unknown User";
        public string Email { get; private set; } = "Unknown Email";
  
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
            Username = HttpContext.Session.GetString("Username") ?? "Unknown User";
            Email = HttpContext.Session.GetString("Email") ?? "Unknown Email";
            // Load confirmed purchases from the database.
            PurchasedProducts = _context.PurchaseRecords
                .Where(p => p.InstitutionId == institutionId)
                .ToList();        

            // If an edit is requested, load the product for editing.
            if (editId.HasValue)
            {
                NewProduct = _context.Products.FirstOrDefault(p => p.Product_id == editId) ?? new Product();
           
            }
       
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

        public IActionResult OnPostAddPendingPurchase(string Name, string Amount, int Quantity, decimal? Price)
        {
            int? institutionId = HttpContext.Session.GetInt32("InstitutionId");
            if (institutionId == null)
                return BadRequest("Institution ID not found in session");

            // Create a pending purchase record
            var purchaseRecord = new PurchaseRecord
            {
                Name = Name,
                Amount = Amount,
                Quantity = Quantity,
                Price = Price,
                InstitutionId = institutionId.Value
            };

            // Retrieve existing pending purchases from session
            var pending = HttpContext.Session.GetObjectFromJson<List<PurchaseRecord>>("PendingPurchases") ?? new List<PurchaseRecord>();

            // Add the new purchase to session storage
            pending.Add(purchaseRecord);
            HttpContext.Session.SetObjectAsJson("PendingPurchases", pending);

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
            int? institutionId = HttpContext.Session.GetInt32("InstitutionId");
            if (institutionId == null)
            {
                return RedirectToPage(); // Reload the same page if InstitutionId is missing
            }

            var product = _context.Products.FirstOrDefault(p => p.Product_id == productId);
            if (product != null)
            {
                var purchaseRecord = new PurchaseRecord
                {
                    Name = product.Name,
                    Amount = null, // Optional
                    Quantity = product.Quantity,
                    Price = product.Price,
                    InstitutionId = institutionId.Value
                };

                var pending = HttpContext.Session.GetObjectFromJson<List<PurchaseRecord>>("PendingPurchases")
                              ?? new List<PurchaseRecord>();

                pending.Add(purchaseRecord);
                HttpContext.Session.SetObjectAsJson("PendingPurchases", pending);
            }

            return RedirectToPage(); // Reload the page after submission
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
    }
}