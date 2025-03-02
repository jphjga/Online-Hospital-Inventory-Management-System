using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Njenga.Data;
using Njenga.Models;
using System;
using System.Collections.Generic;
using System.Linq;

public class SalesModel : PageModel
{
    private readonly DatabaseContext _context;

    // For the right column: existing sales records (unchanged)
    public List<SalesRecord> SalesRecords { get; set; } = new List<SalesRecord>();

    // For the left column: products for sale (using a view model)
    public List<ProductSaleViewModel> Products { get; set; } = new List<ProductSaleViewModel>();

    public SalesModel(DatabaseContext context)
    {
        _context = context;
    }

    public void OnGet()
    {
        int? institutionId = HttpContext.Session.GetInt32("InstitutionId");
        if (institutionId.HasValue)
        {
            // Load existing sales records (right column)
            SalesRecords = _context.SalesRecords
                                   .Where(s => s.InstitutionId == institutionId.Value)
                                   .OrderByDescending(s => s.Date_Time)
                                   .ToList();

            // Load products for sale (left column)
            Products = _context.Products
                .Where(p => p.InstitutionId == institutionId.Value)
                .Select(p => new ProductSaleViewModel
                {
                    Product_id = p.Product_id,
                    Name = p.Name,
                    Quantity = p.Quantity,
                    AmountToSell = 0  // default to 0 (i.e. no sale)
                })
                .ToList();
        }
        else
        {
            SalesRecords = new List<SalesRecord>();
            Products = new List<ProductSaleViewModel>();
        }
    }

    public IActionResult OnPost()
    {
        int? institutionId = HttpContext.Session.GetInt32("InstitutionId");
        string? userFullName = HttpContext.Session.GetString("Username");

        if (!institutionId.HasValue || string.IsNullOrEmpty(userFullName))
        {
            return RedirectToPage("Index");
        }

        // Determine the number of product rows posted
        int productCount = 0;
        while (Request.Form.ContainsKey($"Products[{productCount}].Product_id"))
        {
            productCount++;
        }

        for (int i = 0; i < productCount; i++)
        {
            string? productIdStr = Request.Form[$"Products[{i}].Product_id"];
            string? amountToSellStr = Request.Form[$"Products[{i}].AmountToSell"];

            if (string.IsNullOrEmpty(productIdStr) || string.IsNullOrEmpty(amountToSellStr))
                continue;

            int productId = int.Parse(productIdStr);
            int amountToSell = int.Parse(amountToSellStr);

            // Skip if no sale is made
            if (amountToSell <= 0)
                continue;

            // Find the product in the database for the current institution
            var product = _context.Products.FirstOrDefault(p =>
                                p.Product_id == productId &&
                                p.InstitutionId == institutionId.Value);

            if (product == null)
            {
                ModelState.AddModelError(string.Empty, $"Product with ID {productId} not found.");
                return Page();
            }

            // Validate that there is enough stock
            if (product.Quantity < amountToSell)
            {
                ModelState.AddModelError(string.Empty, $"Not enough stock available for {product.Name}.");
                return Page();
            }

            // Update product quantity (current - amountToSell)
            product.Quantity -= amountToSell;
            _context.Products.Update(product);

            // Record the sale in the SalesRecord table
            var salesRecord = new SalesRecord
            {
                InstitutionId = institutionId.Value,
                Date_Time = DateTime.Now,
                User = userFullName,
                Count = amountToSell
            };
            _context.SalesRecords.Add(salesRecord);
        }

        _context.SaveChanges();
        return RedirectToPage();  // Return to the same page after processing
    }

    public class ProductSaleViewModel
    {
        public int Product_id { get; set; }
        public string Name { get; set; } = "";
        public int Quantity { get; set; }
        public int AmountToSell { get; set; }
    }
}
