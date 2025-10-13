using CsvProcessor.Shared.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CsvProcessor.Shared.Models;

public class LoginModel : PageModel
{
    private readonly CsvDbContext _context;
    private readonly PasswordHasher<User> _hasher = new();

    public LoginModel(CsvDbContext context)
    {
        _context = context;
    }

    [BindProperty] public string Username { get; set; } = string.Empty;
    [BindProperty] public string Password { get; set; } = string.Empty;
    [BindProperty] public bool RememberMe { get; set; }
    public string? Message { get; set; }

    public void OnGet()
    {
        // If user is already logged in
        if (HttpContext.Session.GetString("Username") != null)
        {
            Response.Redirect("/Index");
        }
        else
        {
            // Check RememberMe cookie
            var cookieUser = Request.Cookies["AppUser"];
            if (!string.IsNullOrEmpty(cookieUser))
            {
                HttpContext.Session.SetString("Username", cookieUser);
                Response.Redirect("/Index");
            }
        }
    }

    public IActionResult OnPost()
    {
        var user = _context.Users.FirstOrDefault(u => u.UserName == Username);
        if (user == null)
        {
            Message = "Invalid username or password.";
            return Page();
        }

        var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, Password);
        if (result == PasswordVerificationResult.Failed)
        {
            Message = "Invalid username or password.";
            return Page();
        }

        // ✅ Store session
        HttpContext.Session.SetString("Username", Username);

        // ✅ Store cookie if RememberMe checked
        if (RememberMe)
        {
            CookieOptions options = new()
            {
                Expires = DateTime.Now.AddDays(7), // valid for 1 week
                HttpOnly = true,
                Secure = false // set to true in HTTPS
            };
            Response.Cookies.Append("AppUser", Username, options);
        }

        return RedirectToPage("/Index");
    }
}
