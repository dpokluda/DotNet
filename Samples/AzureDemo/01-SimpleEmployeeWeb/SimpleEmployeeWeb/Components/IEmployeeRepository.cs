// -------------------------------------------------------------------------
// <copyright file="IEmployeeRepository.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -------------------------------------------------------------------------

namespace Test.Components;

public interface IEmployeeRepository
{
    List<Employee> GetEmployees();
}