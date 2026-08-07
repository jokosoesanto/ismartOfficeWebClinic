using System;
using Clinic.Domain.Enums;

namespace Clinic.Domain.Entities.System
{
    public class FileMetadata
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        
        public StorageModule Module { get; set; }
        
        // E.g. PatientId, DoctorId, InvoiceId
        public Guid? EntityId { get; set; }
        
        public string OriginalFileName { get; set; } = null!;
        
        // This is typically the GUID-based name like 4b9a0f.webp
        public string StoredFileName { get; set; } = null!;
        
        // Relative path e.g. "Storage/Patient/2026/08/07"
        public string RelativePath { get; set; } = null!;
        
        public string Extension { get; set; } = null!;
        
        public string MimeType { get; set; } = null!;
        
        public int? Width { get; set; }
        public int? Height { get; set; }
        
        public long FileSize { get; set; }
        
        // Used to identify exact duplicates
        public string ContentHash { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Guid? CreatedBy { get; set; }
        
        // Soft delete properties
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public Guid? DeletedBy { get; set; }
    }
}
