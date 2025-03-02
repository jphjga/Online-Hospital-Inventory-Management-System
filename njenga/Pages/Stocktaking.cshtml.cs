using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Njenga.Data;
using Njenga.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace njenga.Pages
{
    public class StocktakingModel(DatabaseContext context) : PageModel
    {
        private readonly DatabaseContext _context = context;

        // For the right column: existing stock-taking records
        public List<StockTaking> StockTakings { get; set; } = [];

        // For the left column: products for the stock-taking exercise
        public List<ProductStockViewModel> Products { get; set; } = [];

        public void OnGet()
        {
            int? institutionId = HttpContext.Session.GetInt32("InstitutionId");
            if (institutionId.HasValue)
            {
                // Retrieve stock-taking records for the current institution (right column)
                StockTakings = [.. _context.StockTakings
                                       .Where(s => s.InstitutionId == institutionId.Value)
                                       .OrderByDescending(s => s.DateTime)];

                // Retrieve products for stock-taking (left column)
                Products = [.. _context.Products
                    .Where(p => p.InstitutionId == institutionId.Value)
                    .Select(p => new ProductStockViewModel
                    {
                        Product_id = p.Product_id,
                        Name = p.Name,
                        Quantity = p.Quantity,
                        NewQuantity = p.Quantity, // default value equals current quantity
                        RemoveExpired = false
                    })];
            }
            else
            {
                StockTakings = [];
                Products = [];
            }
        }

        public IActionResult OnPost()
        {
            int? institutionId = HttpContext.Session.GetInt32("InstitutionId");
            if (!institutionId.HasValue)
            {
                return RedirectToPage("Index");
            }

            // Determine the number of products posted using a loop over indices
            int productCount = 0;
            while (Request.Form.ContainsKey($"Products[{productCount}].Product_id"))
            {
                productCount++;
            }

            // Update product quantities based on submitted form data
            for (int i = 0; i < productCount; i++)
            {
                string? productIdStr = Request.Form[$"Products[{i}].Product_id"];
                string? newQuantityStr = Request.Form[$"Products[{i}].NewQuantity"];
                string? removeExpiredStr = Request.Form[$"Products[{i}].RemoveExpired"];

                if (productIdStr != null && newQuantityStr != null)
                {
                    int productId = int.Parse(productIdStr);
                    int newQuantity = int.Parse(newQuantityStr);
                    // For checkboxes, if not checked, the value won't be present
                    bool removeExpired = removeExpiredStr == "true";

                    // Find the product in the database and update it
                    var product = _context.Products.FirstOrDefault(p => p.Product_id == productId);
                    if (product != null)
                    {
                        product.Quantity = removeExpired ? 0 : newQuantity;
                    }
                }
            }
            _context.SaveChanges();

            // Record the stock-taking exercise in the StockTaking table
            var stockTakingRecord = new StockTaking
            {
                InstitutionId = institutionId.Value,
                DateTime = DateTime.Now,
                User = HttpContext.Session.GetString("Username") ?? "Unknown"
            };
            _context.StockTakings.Add(stockTakingRecord);
            _context.SaveChanges();

            return RedirectToPage();
        }

        public class ProductStockViewModel
        {
            public int Product_id { get; set; }
            public string Name { get; set; } = "";
            public int Quantity { get; set; }
            public int NewQuantity { get; set; }
            public bool RemoveExpired { get; set; }
        }
    }
}