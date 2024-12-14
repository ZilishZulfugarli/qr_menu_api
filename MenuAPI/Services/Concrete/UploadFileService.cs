using MenuAPI.Services.Abstract;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

public class UploadFileService : IUploadFileService
{
    private readonly IWebHostEnvironment _env;

    public UploadFileService(IWebHostEnvironment env)
    {
        _env = env;
    }

    public async Task<string> UploadFileAsync(IFormFile file, string folderName)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("File is null or empty");

        var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

        var folderPath = Path.Combine(_env.WebRootPath, folderName);

        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        var filePath = Path.Combine(folderPath, uniqueFileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return Path.Combine(folderName, uniqueFileName).Replace("\\", "/");
    }

    public async Task<List<string>> UploadFilesAsync(IEnumerable<IFormFile> files, string folderName)
    {
        var filePaths = new List<string>();

        foreach (var file in files)
        {
            var filePath = await UploadFileAsync(file, folderName);
            filePaths.Add(filePath);
        }

        return filePaths;
    }
}
