using Microsoft.AspNetCore.Mvc.RazorPages;
using Njenga.Data;
using Njenga.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace njenga.Pages
{
    public class ReportsModel(DatabaseContext context) : PageModel
    {
        private readonly DatabaseContext _context = context;
        public List<SalesRecord> SalesRecords { get; set; } = [];
        public int TotalSales { get; set; }
        public List<SalesTrend> SalesTrends { get; set; } = [];
        public List<InventorySummaryItem> InventorySummaries { get; set; } = [];

        public List<Product> Products { get; set; } = [];
        public List<PurchaseTrend> PurchaseTrends { get; set; } = [];
        public List<PurchaseRecord> RecentPurchases { get; set; } = [];
        public MostPurchasedProduct MostPurchased { get; set; } = new MostPurchasedProduct();

        public void OnGet()
        {
            int? institutionId = HttpContext.Session.GetInt32("InstitutionId");
            if (institutionId.HasValue)
            {
                // Existing Sales and Inventory logic...
                SalesRecords = [.. _context.SalesRecords
                .Where(s => s.InstitutionId == institutionId.Value)
                .OrderByDescending(s => s.Date_Time)];

                TotalSales = SalesRecords.Sum(s => s.Count);

                SalesTrends = [.. _context.SalesRecords
                .Where(s => s.InstitutionId == institutionId.Value)
                .GroupBy(s => new { s.Date_Time.Year, s.Date_Time.Month })
                .Select(g => new SalesTrend
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Total = g.Sum(s => s.Count)
                })
                .OrderBy(t => t.Year)
                .ThenBy(t => t.Month)];

                InventorySummaries = [.. _context.Products
                .Where(p => p.InstitutionId == institutionId.Value)
                .GroupBy(p => p.Category)
                .Select(g => new InventorySummaryItem
                {
                    Category = g.Key,
                    TotalStock = g.Sum(p => p.Quantity),
                    ProductCount = g.Count()
                })];

                Products = [.. _context.Products.Where(p => p.InstitutionId == institutionId.Value)];
                // NEW: Aggregate purchase trends by month
                PurchaseTrends = [.. _context.PurchaseRecords.Where(pr => pr.InstitutionId == institutionId.Value).GroupBy(pr => new { pr.Date_Time.Year, pr.Date_Time.Month }).Select(g => new PurchaseTrend { Year = g.Key.Year, Month = g.Key.Month, Total = g.Sum(pr => pr.Quantity ?? 0) }).OrderBy(t => t.Year).ThenBy(t => t.Month)];

                // NEW: Load recent purchases (last 10 records, for example)
                RecentPurchases = [.. _context.PurchaseRecords
                .Where(pr => pr.InstitutionId == institutionId.Value)
                .OrderByDescending(pr => pr.Id) // or use a Date_Time field if available
                .Take(10)];

                // NEW: Determine the most purchased product
                var mostPurchasedData = _context.PurchaseRecords
                    .Where(pr => pr.InstitutionId == institutionId.Value)
                    .GroupBy(pr => pr.Name)
                    .Select(g => new { ProductName = g.Key, TotalQuantity = g.Sum(pr => pr.Quantity ?? 0) })
                    .OrderByDescending(x => x.TotalQuantity)
                    .FirstOrDefault();
                if (mostPurchasedData != null)
                {
                    MostPurchased = new MostPurchasedProduct
                    {
                        Name = mostPurchasedData.ProductName,
                        TotalQuantity = mostPurchasedData.TotalQuantity
                    };
                }
            }
            else
            {
                SalesRecords = [];
                SalesTrends = [];
                InventorySummaries = [];
                PurchaseTrends = [];
                RecentPurchases = [];
                MostPurchased = new MostPurchasedProduct();
            }
        }
    }

    public class SalesTrend
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int Total { get; set; }
    }

    public class InventorySummaryItem
    {
        public string Category { get; set; } = "";
        public int TotalStock { get; set; }
        public int ProductCount { get; set; }
    }

    // NEW: Aggregation for Purchase Trends
    public class PurchaseTrend
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int Total { get; set; }
    }

    // NEW: Model for the Most Purchased Product
    public class MostPurchasedProduct
    {
        public string Name { get; set; } = "";
        public int TotalQuantity { get; set; }
    }
}