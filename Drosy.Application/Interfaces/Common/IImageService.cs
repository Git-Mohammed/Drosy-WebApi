using Microsoft.AspNetCore.Http;

namespace Drosy.Application.Interfaces.Common
{
    public interface IImageService
    {
        /// <summary>
        /// Saves an uploaded image to the designated folder and returns its relative path.
        /// </summary>
        /// <param name="file">The image file to save.</param>
        /// <param name="folder">The target folder (e.g., "logos").</param>
        /// <returns>The relative path to the saved image.</returns>
        Task<string> SaveImageAsync(IFormFile file, string folder);

        /// <summary>
        /// Deletes an image from the file system.
        /// </summary>
        /// <param name="relativePath">The relative path of the image to delete.</param>
        void DeleteImage(string relativePath);
    }
}
