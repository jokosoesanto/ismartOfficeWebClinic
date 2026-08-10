using System;
using Clinic.Domain.Entities.System;

namespace Clinic.Domain.Entities.MasterData
{
    public class TreatmentCatalog
    {
        public Guid Id { get; set; }
        public string TreatmentCode { get; set; } = string.Empty;
        public string TreatmentName { get; set; } = string.Empty;
        
        public Guid CategoryId { get; set; }
        public virtual TreatmentCategory? Category { get; set; }
        
        public Guid SubCategoryId { get; set; }
        public virtual TreatmentSubCategory? SubCategory { get; set; }
        
        public Guid ServiceTypeId { get; set; }
        public virtual MasterReference? ServiceType { get; set; }
        
        public decimal DefaultPrice { get; set; }
        public int DurationInMinutes { get; set; }
        
        public bool RequiresTooth { get; set; }
        public bool RequiresSurface { get; set; }
        
        public string? Description { get; set; }
        
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Guid? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
        public Guid? DeletedBy { get; set; }
    }
}
