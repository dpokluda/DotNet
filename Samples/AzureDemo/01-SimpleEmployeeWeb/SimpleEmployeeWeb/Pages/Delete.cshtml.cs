// -------------------------------------------------------------------------
// <copyright file="Delete.cshtml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -------------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Test.Components;

namespace SimpleEmployeeWeb.Pages;

public class DeleteModel : PageModel
{
    private readonly IEmployeeRepository _repository;
    private readonly ILogger<DeleteModel> _logger;

    public DeleteModel(IEmployeeRepository repository, ILogger<DeleteModel> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public IActionResult OnGet(int id)
    {
        Id = id.ToString();
        var employee = _repository.GetEmployees().FirstOrDefault(e => e.Id == id);
        if (employee == null)
        {
            return RedirectToPage("Error", new { message = "Unknown employee id."});
        }
        
        Name = employee.Name;
        return null;
    }

    public IActionResult OnPost()
    {
        int id;
        if (!Int32.TryParse(Id, out id))
        {
            return RedirectToPage("Error", new { message = "Unknown employee id."});
        }

        _repository.GetEmployees().RemoveAll(e => e.Id == id);
        return RedirectToPage("Index");
    }
    
    [BindProperty(SupportsGet= true )]
    public string Id { get; set; }
    
    [BindProperty]
    public string Name { get; set; }
}