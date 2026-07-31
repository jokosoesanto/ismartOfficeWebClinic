# Root Cause Analysis

## Question
Mengapa seluruh halaman masih terlihat identik meskipun puluhan template baru telah berhasil di-render tanpa exception?

## Answer (Root Cause)
**Layout menimpa Body.**

## Objective Proof
The `_RegionLayout.cshtml` file acts as the master structure for all templates. It contains the following defensive rendering logic:

```razor
@if (hasAnyComponent)
{
    if (Model.Composition.Center.Any())
    {
        IgnoreBody();
        foreach (var comp in Model.Composition.Center)
        {
            @await ComponentRegistry.RenderComponentAsync(Component, comp, Model.Data)
        }
    }
    else
    {
        @RenderBody();
    }
}
```

Every single Controller in the application (such as `PatientController`, `AppointmentController`, etc.) is still hardcoded to inject a legacy Prototype ViewComponent into `Model.Composition.Center` via the `UIMetadata` object. 

Because `Model.Composition.Center` is never empty, the `if` condition always evaluates to `true`. Consequently, `IgnoreBody()` is executed every time, permanently discarding the `RenderBody()` call that would have displayed our newly created Physical Razor Templates. Instead, the legacy Prototype ViewComponents are rendered, resulting in the old UI.

This design is a feature, not a bug—the UI Framework prioritizes Metadata Components over physical Razor Views by design. To use physical templates as the primary UI, the Metadata must not inject conflicting Center components, OR the layout must be adjusted to allow both.
