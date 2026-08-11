using System;
using System.Collections.Generic;
using System.Linq;
using Clinic.Domain.Entities.System;

namespace Clinic.Infrastructure.Data.Seeders
{
    public static class SystemSeeder
    {
        public static void Seed(AppDbContext context)
        {
            SeedMasterReferences(context);
            SeedNumberSequences(context);
            context.SaveChanges();
        }

        private static void SeedMasterReferences(AppDbContext context)
        {
            var systemReferences = new List<MasterReference>();

            // Helper to generate quickly
            void AddRefs(string category, string[] items)
            {
                int order = 1;
                foreach (var item in items)
                {
                    systemReferences.Add(new MasterReference
                    {
                        Id = Guid.NewGuid(),
                        Category = category,
                        Code = item.ToUpper().Replace(" ", "_"),
                        Name = item,
                        SortOrder = order++,
                        IsSystem = true,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }

            AddRefs("Religion", new[] { "Islam", "Protestantism", "Catholicism", "Hinduism", "Buddhism", "Confucianism", "Other" });
            AddRefs("BloodType", new[] { "A+", "A-", "B+", "B-", "AB+", "AB-", "O+", "O-", "Unknown" });
            AddRefs("Gender", new[] { "Male", "Female", "Other", "Unknown" });
            AddRefs("Relationship", new[] { "Self", "Spouse", "Child", "Parent", "Sibling", "Other" });
            AddRefs("Nationality", new[] { "Domestic", "Foreign" });
            AddRefs("Language", new[] { "Indonesian", "English", "Other" });
            AddRefs("Occupation", new[] { "Student", "Employee", "Self-employed", "Unemployed", "Retired", "Other" });
            AddRefs("Education", new[] { "None", "Primary", "Secondary", "Bachelor", "Master", "Doctorate", "Other" });
            AddRefs("PatientCategory", new[] { "General", "VIP", "VVIP" });
            AddRefs("PatientStatus", new[] { "Active", "Inactive", "Deceased" });
            AddRefs("MaritalStatus", new[] { "Single", "Married", "Divorced", "Widowed" });
            AddRefs("Country", new[] { "Indonesia", "Malaysia", "Singapore" });
            AddRefs("Province", new[] { "DKI Jakarta", "West Java", "Central Java", "East Java", "Bali" });
            AddRefs("PreferredCommunication", new[] { "Phone", "WhatsApp", "SMS", "Email" });
            AddRefs("Currency", new[] { "IDR", "USD", "EUR", "SGD", "MYR", "AUD", "JPY" });
            AddRefs("InsuranceGroup", new[] { "PPO", "HMO", "Medicare", "Medicaid", "Commercial", "Other" });

            foreach (var r in systemReferences)
            {
                if (!context.MasterReferences.Any(x => x.Category == r.Category && x.Code == r.Code))
                {
                    context.MasterReferences.Add(r);
                }
            }
        }

        private static void SeedNumberSequences(AppDbContext context)
        {
            if (!context.NumberSequences.Any(x => x.SequenceCode == "MR"))
            {
                context.NumberSequences.Add(new NumberSequence
                {
                    Id = Guid.NewGuid(),
                    SequenceCode = "MR",
                    Prefix = "MR",
                    Padding = 10,
                    ResetPolicy = Clinic.Domain.Enums.SequenceResetPolicy.Never,
                    IncrementStep = 1,
                    CurrentValue = 0,
                    CreatedAt = DateTime.UtcNow
                });
            }
        }
    }
}
