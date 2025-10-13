using CsvProcessor.Shared.Data;
using CsvProcessor.Shared.Models;
using CsvProcessor.Shared.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Pages
{

    public class IndexModel : PageModel
    {
        private readonly CsvDbContext _dbContext;
        private readonly IFileService _fileService;

        public IndexModel(CsvDbContext dbContext, IFileService fileService)
        {
            _dbContext = dbContext;
            _fileService = fileService;
        }

        [BindProperty]
        public string FilePath { get; set; } = string.Empty;
        public string? LoggedUser { get; set; }


        public List<FileProcess> FileProcesses { get; set; } = new List<FileProcess>();

        public async Task<IActionResult> OnGetAsync()
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
                return RedirectToPage("/Login");
            LoggedUser = username;
            // Load latest 20 files
            FileProcesses = await _dbContext.FileProcesses
                                .OrderByDescending(f => f.CreatedAt)
                                .Take(20)
                                .ToListAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(FilePath))
            {
                ModelState.AddModelError("", "Please enter a valid file path.");
                await OnGetAsync();
                return Page();
            }

            // For demo, assume UserId = Guid.Empty
            var userId = Guid.Empty;

            await _fileService.StartProcessingFileAsync(userId, FilePath);

            // Reload files list after submission
            await OnGetAsync();

            return Page();
        }
    }
}
