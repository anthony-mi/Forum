using Forum.Data;
using Forum.Models.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Forum.Services
{
    public class ImagesManager
    {
        private readonly IWebHostEnvironment _environment;
        private readonly string _imagesStorage;

        public ImagesManager(IWebHostEnvironment environment)
        {
            _environment = environment;
            _imagesStorage = Path.Combine(_environment.WebRootPath, "Resources", "Images");
        }

        public async Task RemoveAsync(Image image)
        {
            try
            {
                string path = Path.Combine(_imagesStorage, image.Filename);

                if(File.Exists(path))
                {
                    File.Delete(path);
                }
            }
            catch { }
        }

        public async Task<Image> CreateAsync(IFormFile file)
        {
            Image createdImage = null;

            try
            {
                string fileName = GenerateUniqueFileName(file.FileName);
                string filePath = Path.Combine(_imagesStorage, fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create/*, FileAccess.ReadWrite, FileShare.Read*/))
                {
                    await file.CopyToAsync(fileStream);
                }

                createdImage = new Image
                {
                    Filename = fileName
                };
            }
            catch {  }

            return createdImage;
        }

        private string GenerateUniqueFileName(string filename)
        {
            string path = Path.Combine(_imagesStorage, filename);

            if (!File.Exists(path))
            {
                return filename;
            }

            string preExtensionPart = Path.GetFileNameWithoutExtension(filename);
            string fileExt = Path.GetExtension(filename);

            for (var i = 2; ; i++)
            {
                path = Path.Combine(_imagesStorage, preExtensionPart + $"_{i}" + fileExt);

                if (!File.Exists(path))
                {
                    return preExtensionPart + $"_{i}" + fileExt;
                }
            }
        }
    }
}
