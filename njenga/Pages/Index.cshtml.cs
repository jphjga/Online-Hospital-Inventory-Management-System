using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Njenga.Data;
using Njenga.Models;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace Njenga.Pages
{
    public class IndexModel(DatabaseContext context) : PageModel
    {
        private readonly DatabaseContext _context = context;

        [BindProperty]
        public string Email { get; set; } = string.Empty;

        [BindProperty]
        public string Password { get; set; } = string.Empty;

        [BindProperty]
        public int InstitutionId { get; set; }

        public IList<Institution> Institutions { get; set; } = [];
        public string? ErrorMessage { get; set; }

        public void OnGet()
        {
            Institutions = [.. _context.Institutions];
        }

        public IActionResult OnPost()
        {
            var user = _context.Accounts.FirstOrDefault(a => a.Email == Email && a.Password == Password && a.InstitutionId == InstitutionId);

            if (user != null)
            {
                HttpContext.Session.SetInt32("InstitutionId", InstitutionId);
                return new JsonResult(new { success = true, redirectUrl = Url.Page("Dashboard") });
            }

            // Return an error response in JSON format
            return new JsonResult(new { success = false, errorMessage = "Invalid login credentials." });
        }


    }
}