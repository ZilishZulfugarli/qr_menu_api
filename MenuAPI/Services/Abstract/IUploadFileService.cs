using System;
namespace MenuAPI.Services.Abstract
{
        public interface IUploadFileService
        {
            Task<string> UploadFileAsync(IFormFile file, string folderName);
            Task<List<string>> UploadFilesAsync(IEnumerable<IFormFile> files, string folderName);
        }
}

