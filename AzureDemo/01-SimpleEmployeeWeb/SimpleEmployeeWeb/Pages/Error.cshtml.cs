using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SimpleEmployeeWeb.Pages;

[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
[IgnoreAntiforgeryToken]
public class ErrorModel : PageModel
{
    private readonly ILogger<ErrorModel> _logger;

    public ErrorModel(ILogger<ErrorModel> logger)
    {
        _logger = logger;
    }

    public void OnGet(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            Error = "An error occurred while processing your request.";
        }
        else
        {
            Error = message;
        }
    }
    
    [BindProperty(SupportsGet = true)]
    public string Error { get; set; }
}

