using System;

namespace Clinic.Domain.Entities.MasterData
{
    public class TreatmentSubCategory
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        
        public Guid CategoryId { get; set; }
        public TreatmentCategory Category { get; set; } = null!;
        
        public string SubCategoryCode { get; set; } = null!;
        public string SubCategoryName { get; set; } = null!;
        public string? Description { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Guid? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public Guid? DeletedBy { get; set; }
    }
}
