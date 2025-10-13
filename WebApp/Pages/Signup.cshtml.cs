using CsvProcessor.Shared.Data;
using CsvProcessor.Shared.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class SignupModel : PageModel
{
    private readonly CsvDbContext _context;
    private readonly PasswordHasher<User> _hasher = new();

    public SignupModel(CsvDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public string Username { get; set; } = string.Empty;

    [BindProperty]
    public string Password { get; set; } = string.Empty;

    public string? Message { get; set; }

    public void OnGet() { }

    public IActionResult OnPost()
    {
        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
        {
            Message = "Username and Password are required.";
            return Page();
        }

        if (_context.Users.Any(u => u.UserName == Username))
        {
            Message = "Username already exists.";
            return Page();
        }

        var user = new User { UserName = Username };
        user.PasswordHash = _hasher.HashPassword(user, Password);

        _context.Users.Add(user);
        _context.SaveChanges();

        Message = "Signup successful! You can now login.";
        return RedirectToPage("/Login");
    }
}
