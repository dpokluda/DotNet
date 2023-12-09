// -------------------------------------------------------------------------
// <copyright file="Employee.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -------------------------------------------------------------------------

namespace Test.Components;

public class Employee
{
    public Employee(int id, string name, string email, string department, 
                    bool isMacUser = false,
                    bool isWindowsUser = false)
    {
        Name = name;
        Email = email;
        Department = department;
        IsMacUser = isMacUser;
        IsWindowsUser = isWindowsUser;
        Id = id;
    }
    
    public Employee()
    {}

    public int Id { get; set; }
    
    public string PictureUrl { get; set; }
    
    public string Name { get; set; }
    
    public string Email { get; set; }
    
    public string Department { get; set; }
    
    public bool IsMacUser { get; set; }
    public bool IsWindowsUser { get; set; }
}