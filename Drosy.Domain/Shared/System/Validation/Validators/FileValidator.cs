using Drosy.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Drosy.Domain.Shared.System.Validation.Rules;

namespace Drosy.Domain.Shared.System.Validation.Validators;

public static class FileValidator
{
    public static bool HasAllowedExtension(IFormFile file, MediaType type)
    {
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

        return type switch
        {
            MediaType.Image => MediaValidationRules.AllowedImageExtensions.Contains(extension),
            MediaType.Video => MediaValidationRules.AllowedVideoExtensions.Contains(extension),
            MediaType.Audio => MediaValidationRules.AllowedAudioExtensions.Contains(extension),
            _ => false
        };
    }

    public static bool IsWithinAllowedSize(IFormFile file, MediaType type)
    {
        if (file == null) return false;

        long maxSize = type switch
        {
            MediaType.Image => MediaValidationRules.MaxImageSize,
            MediaType.Video => MediaValidationRules.MaxVideoSize,
            MediaType.Audio => MediaValidationRules.MaxAudioSize,
            _ => 0
        };

        return file.Length <= maxSize;
    }
}
