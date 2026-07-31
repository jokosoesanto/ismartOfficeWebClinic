# Rendering Mode Implementation

## 1. UIMetadata Changes
The `UIMetadata` class was expanded to include an explicit `RenderingMode` enum.
```csharp
public enum RenderingMode
{
    Metadata,
    Template
}
```
The property `Mode` defaults to `RenderingMode.Metadata` to ensure older controllers and modules not participating in the prototype migration remain entirely intact.

## 2. Controller Configuration
All new prototype controllers (`PatientController`, `BillingController`, etc.) were updated to:
1. Set `Mode = RenderingMode.Template`.
2. Clear the `Composition` fields (specifically `Composition.Center`).
3. Cease using ViewComponents like `PrototypeTransaction` as full-page hosts.

Example:
```csharp
var meta = new UIMetadata
{
    Title = "Medical Records",
    ModuleName = "MedicalRecord",
    Mode = RenderingMode.Template
};
return View("Templates/MR_Chart", meta);
```

## 3. Template Cleanup
All physical templates in `Views/Shared/Templates/` were stripped of `Layout = "_RegionLayout";`. They no longer possess knowledge about the layout mechanism or framework intricacies.
