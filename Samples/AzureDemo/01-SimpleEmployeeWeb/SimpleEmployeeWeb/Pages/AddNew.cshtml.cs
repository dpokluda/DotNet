// -------------------------------------------------------------------------
// <copyright file="AddNew.cshtml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -------------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Test.Components;

namespace SimpleEmployeeWeb.Pages;

public class AddNewModel : PageModel
{
    private readonly IEmployeeRepository _repository;
    private readonly IPictureService _pictureService;
    private readonly ILogger<AddNewModel> _logger;

    public AddNewModel(IEmployeeRepository repository, IPictureService pictureService, ILogger<AddNewModel> logger)
    {
        _repository = repository;
        _pictureService = pictureService;
        _logger = logger;
    }
    public void OnGet()
    {
        
    }
    
    public IActionResult OnPost()
    {
        int maxId = 0;
        var employees = _repository.GetEmployees();
        if (employees.Count != 0)
        {
            maxId = employees.Max(e => e.Id);
        }

        var employee = new Employee();
        employee.Id = maxId + 1;
        employee.Name = Name;
        employee.Department = Department;
        employee.Email = Email;
        employee.IsMacUser = IsMacUser;
        employee.IsWindowsUser = IsWindowsUser;

        if (File != null)
        {
            var pictureUrl = _pictureService.UploadEmployeePicture(employee.Id, File);
            employee.PictureUrl = pictureUrl;
        }
        _repository.GetEmployees().Add(employee);
        
        return RedirectToPage("Index");
    }
    
    [BindProperty]
    public IFormFile  File { get; set; }
    
    [BindProperty]
    public string Name { get; set; }
    
    [BindProperty]
    public string Department { get; set; }
    
    [BindProperty]
    public string Email { get; set; }
    
    [BindProperty]
    public bool IsMacUser { get; set; }
    
    [BindProperty]
    public bool IsWindowsUser { get; set; }
    
}