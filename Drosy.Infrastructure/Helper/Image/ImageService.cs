using Drosy.Application.Interfaces.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Drosy.Infrastructure.Helper.Image
{
    public class ImageService : IImageService
    {
        private readonly string _webRootPath;
        private readonly Application.Interfaces.Common.ILogger<ImageService> _logger;

        public ImageService(IWebHostEnvironment env, Application.Interfaces.Common.ILogger<ImageService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            // Fallback if WebRootPath is null
            _webRootPath = env.WebRootPath;

            if (string.IsNullOrWhiteSpace(_webRootPath))
            {
                _webRootPath = Path.Combine(env.ContentRootPath, "wwwroot");

                if (!Directory.Exists(_webRootPath))
                {
                    Directory.CreateDirectory(_webRootPath);
                    _logger.LogWarning("WebRootPath was null. Created fallback wwwroot at: {Path}", _webRootPath);
                }
            }
        }

        public async Task<string> SaveImageAsync(IFormFile file, string folder)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Invalid image file.", nameof(file));

            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            var relativeFolder = Path.Combine("uploads", folder);
            var absoluteFolder = Path.Combine(_webRootPath, relativeFolder);

            if (!Directory.Exists(absoluteFolder))
                Directory.CreateDirectory(absoluteFolder);

            var filePath = Path.Combine(absoluteFolder, fileName);

            try
            {
                await using var stream = new FileStream(filePath, FileMode.Create);
                await file.CopyToAsync(stream);
                _logger.LogInformation("Image saved: {Path}", filePath);
            }
            catch (Exception ex)
            {
                _logger.LogError( "Error saving image: {FileName}",ex, fileName);
                throw;
            }

            return Path.Combine("/", relativeFolder, fileName).Replace("\\", "/");
        }

        public void DeleteImage(string relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
                return;

            var normalizedPath = relativePath.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString());
            var fullPath = Path.Combine(_webRootPath, normalizedPath);

            try
            {
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                    _logger.LogInformation("Image deleted: {Path}", fullPath);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to delete image at path: {Path}", ex, relativePath);
            }
        }
    }
}
