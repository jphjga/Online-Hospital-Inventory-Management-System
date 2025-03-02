using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using Njenga.Data;
using System.Text.Json;
using Njenga.Models;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

public class MainDashModel : PageModel
{
    private readonly DatabaseContext _context;

    public string InstitutionName { get; set; } = "Unknown Institution";
    public string Username { get; private set; } = "Unknown User";
    public string Email { get; private set; } = "Unknown Email";

    public MainDashModel(DatabaseContext context)
    {
        _context = context;
    }

    public void OnGet()
    {
        // Retrieve institutionId from the session.
        int? institutionId = HttpContext.Session.GetInt32("InstitutionId");

        // Retrieve the user's full name and email from the session.
        Username = HttpContext.Session.GetString("Username") ?? "Unknown User";
        Email = HttpContext.Session.GetString("Email") ?? "Unknown Email";

        // If institutionId exists, fetch institution name from the database.
        if (institutionId.HasValue)
        {
            var institution = _context.Institutions
                                      .Where(i => i.InstitutionId == institutionId.Value)
                                      .Select(i => i.Name)
                                      .FirstOrDefault();

            if (!string.IsNullOrEmpty(institution))
            {
                InstitutionName = institution;
            }
        }
    }

    // OnPostUpdatePasswordAsync is used to handle the password update request.
  public async Task<IActionResult> OnPostUpdatePasswordAsync()
{
    try
    {
        // Read the form data from the request
        var userId = Convert.ToInt32(Request.Form["userId"]);
        var institutionId = Convert.ToInt32(Request.Form["institutionId"]);
        var username = Request.Form["username"];
        var email = Request.Form["email"];       
        var newPassword = Request.Form["newPassword"].FirstOrDefault();  // Get the first value as a string
                                                                             // Check if data is valid
            if (userId == 0 || string.IsNullOrEmpty(newPassword))
        {
            return new JsonResult(new { success = false, errorMessage = "Invalid data provided." });
        }

            // Check if the new password is valid
            if (string.IsNullOrEmpty(newPassword) || newPassword.Length < 4)
            {
                return new JsonResult(new { success = false, errorMessage = "Password must be at least 4 characters long." });
            }

            // Find the user by userId
            var user = _context.Accounts.FirstOrDefault(a => a.User_id == userId);
        if (user == null)
        {
            return new JsonResult(new { success = false, errorMessage = "User not found." });
        }

        // Update the user's password (only the password is updated)
        user.Password = newPassword;

        // Save the changes to the database
        await _context.SaveChangesAsync();

        return new JsonResult(new { success = true, message = "Password updated successfully!" });
    }
    catch (Exception ex)
    {
        // Log the error and return failure response
        Console.WriteLine("Error: " + ex.Message);
        return new JsonResult(new { success = false, errorMessage = "An error occurred: " + ex.Message });
    }
}


    // PasswordUpdateRequest class contains all necessary fields for the password update.
    public class PasswordUpdateRequest
    {
        public int UserId { get; set; }
        public int InstitutionId { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? NewPassword { get; set; } // Only password is being updated
    }
    //query centre modal handler
    public IActionResult OnGetSearchProducts(string q)
    {
        if (string.IsNullOrWhiteSpace(q))
        {
            return new JsonResult(new List<object>());
        }

        var results = _context.Products
                        .Where(p => p.Name.Contains(q))
                        .Select(p => new
                        {
                            name = p.Name,
                            description = p.Description,
                            institutionName = p.Institution != null ? p.Institution.Name : "Unknown Institution",
                            institutionLocation = p.Institution != null ? p.Institution.Location : "Unknown Location",
                            institutionAbout = p.Institution != null ? p.Institution.About : "No Information"
                        })
                        .ToList();

        return new JsonResult(results);
    }


}
