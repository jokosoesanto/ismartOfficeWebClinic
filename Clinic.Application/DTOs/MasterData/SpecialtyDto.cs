using System;
using System.ComponentModel.DataAnnotations;

namespace Clinic.Application.DTOs.MasterData
{
    public class SpecialtyDto
    {
        public Guid? Id { get; set; }
        [Required] public string Code { get; set; } = null!;
        [Required] public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
    }
}