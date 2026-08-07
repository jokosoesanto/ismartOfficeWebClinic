namespace Clinic.Web.Models
{
    public class NumberSequenceViewModel
    {
        public string Code { get; set; } = string.Empty;
        public long CurrentValue { get; set; }
        public string ResetPolicy { get; set; } = string.Empty;
        public string? Prefix { get; set; }
        public string? DatePattern { get; set; }
        public int Padding { get; set; }
        public string NextPreview { get; set; } = string.Empty;
    }
}
