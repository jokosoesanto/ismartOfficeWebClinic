using System;

namespace Clinic.Domain.Entities
{
    public class SystemSetting
    {
        public int Id { get; set; }
        public string Key { get; set; } = null!;
        public string Value { get; set; } = null!;
        public DateTime UpdatedAt { get; set; }
    }
}
