// -------------------------------------------------------------------------
// <copyright file="IPictureService.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -------------------------------------------------------------------------

namespace Test.Components;

public interface IPictureService
{
    string UploadEmployeePicture(int employeeId, IFormFile pictureFile);

    string GetEmployeePicture(int employeeId);
}