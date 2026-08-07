using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Clinic.Application.Configurations
{
    public class StorageOptions
    {
        public const string SectionName = "Storage";

        [Required]
        public string RootFolder { get; set; } = "Storage";
        
        [Range(1024, 104857600)] // 1KB to 100MB
        public long MaximumUploadSize { get; set; } = 5242880; // Default 5 MB
        
        [Required]
        [MinLength(1)]
        public List<string> AllowedExtensions { get; set; } = new List<string> { ".jpg", ".jpeg", ".png", ".webp", ".pdf", ".docx", ".doc", ".xlsx", ".xls" };
        
        [Range(10, 100)]
        public int ImageQuality { get; set; } = 80;
        
        [Range(50, 1000)]
        public int ThumbnailSize { get; set; } = 200;
        
        [Range(100, 5000)]
        public int MaxImageDimension { get; set; } = 1920;
    }
}
