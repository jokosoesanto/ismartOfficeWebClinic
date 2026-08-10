using System;
using System.Collections.Generic;
using System.Linq;
using Clinic.Domain.Entities.MasterData;
using Clinic.Domain.Entities.System;
using Clinic.Infrastructure.Data;

namespace Clinic.Infrastructure.Data.Seeders
{
    public static class TreatmentSeeder
    {
        public static void Seed(AppDbContext context)
        {
            SeedNumberSequences(context);
            context.SaveChanges();
            SeedTreatmentCategories(context);
            context.SaveChanges();
            SeedTreatmentSubCategories(context);
            context.SaveChanges();

            // Treatment Catalog Seeding
            if (!context.TreatmentCatalogs.Any())
            {
                // Ensure we have a service type
                var diagnosticServiceType = context.MasterReferences.FirstOrDefault(r => r.Category == "ServiceType" && r.Code == "SRV-DIAG")?.Id;
                if (!diagnosticServiceType.HasValue)
                {
                    var serviceType = new MasterReference
                    {
                        Id = Guid.NewGuid(),
                        Category = "ServiceType",
                        Code = "SRV-DIAG",
                        Name = "Diagnostic",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = Guid.Empty
                    };
                    context.MasterReferences.Add(serviceType);
                    context.SaveChanges();
                    diagnosticServiceType = serviceType.Id;
                }
                
                var endoServiceType = context.MasterReferences.FirstOrDefault(r => r.Category == "ServiceType" && r.Code == "SRV-ENDO")?.Id;
                if (!endoServiceType.HasValue)
                {
                    var serviceType = new MasterReference
                    {
                        Id = Guid.NewGuid(),
                        Category = "ServiceType",
                        Code = "SRV-ENDO",
                        Name = "Endodontics",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = Guid.Empty
                    };
                    context.MasterReferences.Add(serviceType);
                    context.SaveChanges();
                    endoServiceType = serviceType.Id;
                }

                var tDiagnostic = context.TreatmentCategories.FirstOrDefault(c => c.CategoryName == "Diagnostic");
                var tEndodontics = context.TreatmentCategories.FirstOrDefault(c => c.CategoryName == "Endodontics");
                var tsDentalExam = context.TreatmentSubCategories.FirstOrDefault(c => c.SubCategoryName == "Dental Examination");
                var tsRootCanal = context.TreatmentSubCategories.FirstOrDefault(c => c.SubCategoryName == "Root Canal Treatment");

                if (tDiagnostic != null && tsDentalExam != null)
                {
                    context.TreatmentCatalogs.Add(new TreatmentCatalog
                    {
                        Id = Guid.NewGuid(),
                        TreatmentCode = "TRT0001",
                        TreatmentName = "Comprehensive Dental Examination",
                        CategoryId = tDiagnostic.Id,
                        SubCategoryId = tsDentalExam.Id,
                        ServiceTypeId = diagnosticServiceType.Value,
                        DefaultPrice = 50.00m,
                        DurationInMinutes = 30,
                        RequiresTooth = false,
                        RequiresSurface = false,
                        Description = "Comprehensive oral evaluation",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = Guid.Empty
                    });
                }

                if (tEndodontics != null && tsRootCanal != null)
                {
                    context.TreatmentCatalogs.Add(new TreatmentCatalog
                    {
                        Id = Guid.NewGuid(),
                        TreatmentCode = "TRT0002",
                        TreatmentName = "Root Canal - Anterior",
                        CategoryId = tEndodontics.Id,
                        SubCategoryId = tsRootCanal.Id,
                        ServiceTypeId = endoServiceType.Value,
                        DefaultPrice = 500.00m,
                        DurationInMinutes = 60,
                        RequiresTooth = true,
                        RequiresSurface = false,
                        Description = "Endodontic therapy, anterior tooth",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = Guid.Empty
                    });

                    context.TreatmentCatalogs.Add(new TreatmentCatalog
                    {
                        Id = Guid.NewGuid(),
                        TreatmentCode = "TRT0003",
                        TreatmentName = "Root Canal - Premolar",
                        CategoryId = tEndodontics.Id,
                        SubCategoryId = tsRootCanal.Id,
                        ServiceTypeId = endoServiceType.Value,
                        DefaultPrice = 600.00m,
                        DurationInMinutes = 75,
                        RequiresTooth = true,
                        RequiresSurface = false,
                        Description = "Endodontic therapy, premolar tooth",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = Guid.Empty
                    });
                    
                    context.TreatmentCatalogs.Add(new TreatmentCatalog
                    {
                        Id = Guid.NewGuid(),
                        TreatmentCode = "TRT0004",
                        TreatmentName = "Root Canal - Molar",
                        CategoryId = tEndodontics.Id,
                        SubCategoryId = tsRootCanal.Id,
                        ServiceTypeId = endoServiceType.Value,
                        DefaultPrice = 800.00m,
                        DurationInMinutes = 90,
                        RequiresTooth = true,
                        RequiresSurface = false,
                        Description = "Endodontic therapy, molar tooth",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = Guid.Empty
                    });
                }

                context.SaveChanges();
            }
        }

        private static void SeedNumberSequences(AppDbContext context)
        {
            if (!context.NumberSequences.Any(x => x.SequenceCode == "TC"))
            {
                context.NumberSequences.Add(new NumberSequence
                {
                    Id = Guid.NewGuid(),
                    SequenceCode = "TC",
                    Prefix = "TC",
                    Padding = 4,
                    ResetPolicy = Clinic.Domain.Enums.SequenceResetPolicy.Never,
                    IncrementStep = 1,
                    CurrentValue = 0,
                    CreatedAt = DateTime.UtcNow
                });
            }
            if (!context.NumberSequences.Any(x => x.SequenceCode == "TSC"))
            {
                context.NumberSequences.Add(new NumberSequence
                {
                    Id = Guid.NewGuid(),
                    SequenceCode = "TSC",
                    Prefix = "TSC",
                    Padding = 4,
                    ResetPolicy = Clinic.Domain.Enums.SequenceResetPolicy.Never,
                    IncrementStep = 1,
                    CurrentValue = 0,
                    CreatedAt = DateTime.UtcNow
                });
            }
            if (!context.NumberSequences.Any(x => x.SequenceCode == "TRT"))
            {
                context.NumberSequences.Add(new NumberSequence
                {
                    Id = Guid.NewGuid(),
                    SequenceCode = "TRT",
                    Prefix = "TRT",
                    Padding = 4,
                    ResetPolicy = Clinic.Domain.Enums.SequenceResetPolicy.Never,
                    IncrementStep = 1,
                    CurrentValue = 0,
                    CreatedAt = DateTime.UtcNow
                });
            }
        }

        private static void SeedTreatmentCategories(AppDbContext context)
        {
            var categories = new List<(string Name, int Order)>
            {
                ("Diagnostic", 1),
                ("Preventive", 2),
                ("Restorative", 3),
                ("Endodontics", 4),
                ("Periodontics", 5),
                ("Prosthodontics", 6),
                ("Orthodontics", 7),
                ("Oral Surgery", 8),
                ("Implant Services", 9),
                ("Pediatric Dentistry", 10),
                ("Adjunctive General Services", 11),
                ("Radiology", 12)
            };

            var seq = context.NumberSequences.FirstOrDefault(x => x.SequenceCode == "TC");

            foreach (var cat in categories)
            {
                if (!context.TreatmentCategories.Any(x => x.CategoryName == cat.Name && !x.IsDeleted))
                {
                    string newCode = "TC0000";
                    if (seq != null)
                    {
                        seq.CurrentValue += seq.IncrementStep;
                        newCode = $"{seq.Prefix}{seq.CurrentValue.ToString().PadLeft(seq.Padding, '0')}";
                    }

                    context.TreatmentCategories.Add(new TreatmentCategory
                    {
                        Id = Guid.NewGuid(),
                        CategoryCode = newCode,
                        CategoryName = cat.Name,
                        DisplayOrder = cat.Order,
                        IsActive = true,
                        IsDeleted = false,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }
        }

        private static void SeedTreatmentSubCategories(AppDbContext context)
        {
            var diagCategory = context.TreatmentCategories.FirstOrDefault(x => x.CategoryName == "Diagnostic" && !x.IsDeleted);
            var endoCategory = context.TreatmentCategories.FirstOrDefault(x => x.CategoryName == "Endodontics" && !x.IsDeleted);

            var seq = context.NumberSequences.FirstOrDefault(x => x.SequenceCode == "TSC");

            void SeedSubCat(TreatmentCategory? cat, string name, int order)
            {
                if (cat == null) return;

                if (!context.TreatmentSubCategories.Any(x => x.CategoryId == cat.Id && x.SubCategoryName == name && !x.IsDeleted))
                {
                    string newCode = "TSC0000";
                    if (seq != null)
                    {
                        seq.CurrentValue += seq.IncrementStep;
                        newCode = $"{seq.Prefix}{seq.CurrentValue.ToString().PadLeft(seq.Padding, '0')}";
                    }

                    context.TreatmentSubCategories.Add(new TreatmentSubCategory
                    {
                        Id = Guid.NewGuid(),
                        CategoryId = cat.Id,
                        SubCategoryCode = newCode,
                        SubCategoryName = name,
                        DisplayOrder = order,
                        IsActive = true,
                        IsDeleted = false,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }

            SeedSubCat(diagCategory, "Dental Examination", 1);
            SeedSubCat(diagCategory, "Radiographic Examination", 2);

            SeedSubCat(endoCategory, "Root Canal Treatment", 1);
            SeedSubCat(endoCategory, "Pulp Therapy", 2);
            SeedSubCat(endoCategory, "Endodontic Retreatment", 3);
        }
    }
}
