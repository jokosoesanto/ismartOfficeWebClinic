using System;

namespace Clinic.Application.DTOs.MasterData
{
    public class TreatmentCatalogDto
    {
        public Guid Id { get; set; }
        public string TreatmentCode { get; set; } = string.Empty;
        public string TreatmentName { get; set; } = string.Empty;
        
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        
        public Guid SubCategoryId { get; set; }
        public string SubCategoryName { get; set; } = string.Empty;
        
        public Guid ServiceTypeId { get; set; }
        public string ServiceTypeName { get; set; } = string.Empty;
        
        public decimal DefaultPrice { get; set; }
        public int DurationInMinutes { get; set; }
        
        public bool RequiresTooth { get; set; }
        public bool RequiresSurface { get; set; }
        
        public string? Description { get; set; }
        public bool IsActive { get; set; }
    }
}
