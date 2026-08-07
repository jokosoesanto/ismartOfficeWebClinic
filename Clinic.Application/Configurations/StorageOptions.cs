using System;
using System.Collections.Generic;

namespace Clinic.Application.Configurations
{
    public class StorageOptions
    {
        public const string SectionName = "Storage";

        public string RootFolder { get; set; } = "Storage";
        public long MaximumUploadSize { get; set; } = 5242880; // Default 5 MB
        public List<string> AllowedExtensions { get; set; } = new List<string> { ".jpg", ".jpeg", ".png", ".webp", ".pdf", ".docx", ".doc", ".xlsx", ".xls" };
        public int ImageQuality { get; set; } = 80;
        public int ThumbnailSize { get; set; } = 200;
        public int MaxImageDimension { get; set; } = 1920;
    }
}
