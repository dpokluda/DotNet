// -------------------------------------------------------------------------
// <copyright file="LocalFilePictureService.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -------------------------------------------------------------------------

using System.Drawing;
using System.Drawing.Imaging;

namespace Test.Components;

public class LocalFilePictureService : IPictureService
{
    private static readonly string PictureRepository = Environment.CurrentDirectory + "/wwwroot";
    
    public string UploadEmployeePicture(int employeeId, IFormFile pictureFile)
    {
        var image = Image.FromStream(pictureFile.OpenReadStream());
        var resized = new Bitmap(image, new Size(32, 32));
        using var imageStream = new MemoryStream();
        resized.Save(imageStream, ImageFormat.Jpeg);
        var imageBytes = imageStream.ToArray();
        var pictureUrl = $"/thumbnails/profile-{employeeId}.jpg";
        if (File.Exists(PictureRepository + pictureUrl))
        {
            File.Delete(PictureRepository + pictureUrl);
        }
        using (var stream = new FileStream(
                   PictureRepository + pictureUrl, FileMode.Create, FileAccess.Write, FileShare.Write, 4096))
        {
            stream.Write(imageBytes, 0, imageBytes.Length);
        }

        return pictureUrl;
    }

    public string GetEmployeePicture(int employeeId)
    {
        var pictureUrl = $"/thumbnails/profile-{employeeId}.jpg";
        if (File.Exists(PictureRepository + pictureUrl))
        {
            return pictureUrl;
        }
        else
        {
            return null;
        }
    }
}