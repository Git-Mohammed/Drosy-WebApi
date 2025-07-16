using Drosy.Domain.Enums;
using Microsoft.AspNetCore.Http;
using System.Text.RegularExpressions;

namespace Drosy.Domain.Shared.Helpers
{
    public static class ValidationHelper
    {
        public static readonly Regex PhoneRegex = new(@"^\+?[1-9]\d{1,14}$"); // E.164
        public static readonly Regex EmailRegex = new(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");

        public static readonly int MaxNameLength = 50;
        public static readonly int MaxTitleLength = 100;
        public static readonly int MaxDescriptionLength = 500;

        public static readonly long MaxImageSize = 5 * 1024 * 1024; // 5MB
        public static readonly long MaxVideoSize = 50 * 1024 * 1024; // 50MB
        public static readonly long MaxAudioSize = 10 * 1024 * 1024; // 10MB
        public static readonly string[] ImageExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp", ".bmp", ".heic", ".heif" };
        public static readonly string[] VideoExtensions = new[] { ".mp4", ".mov", ".webm", ".mkv", ".avi" };
        public static readonly string[] AudioExtensions = new[] { ".mp3", ".wav", ".ogg", ".aac" };
     
        public static bool HasAllowedExtension(IFormFile file, MediaType type)
        {
            var extension = Path.GetExtension(file.FileName).ToLower();

            return type switch
            {
                MediaType.Image => ImageExtensions.Contains(extension),
                MediaType.Video => VideoExtensions.Contains(extension),
                MediaType.Audio => AudioExtensions.Contains(extension),
                _ => false
            };
        }

        public static bool IsWithinAllowedSize(IFormFile file, MediaType type)
        {
            if (file == null)
                return false;

            long maxSize = type switch
            {
                MediaType.Image => MaxImageSize,
                MediaType.Video => MaxVideoSize,
                MediaType.Audio => MaxAudioSize,
                _ => 0
            };

            return file.Length <= maxSize;
        }
    }
}
