using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Test.Components;

namespace SimpleEmployeeWeb.Pages;

public class IndexModel : PageModel
{
    private readonly IEmployeeRepository _repository;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(IEmployeeRepository repository, ILogger<IndexModel> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public List<Employee> Employees { get; private set; } 
        
    public void OnGet()
    {
        Employees = _repository.GetEmployees();
    }
}
