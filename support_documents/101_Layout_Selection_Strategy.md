# Layout Selection Strategy

## The Centralized Approach
To ensure Razor Templates do not require intimate knowledge of the framework, layout selection is now performed globally and centrally within `_ViewStart.cshtml`.

## Logic
```razor
@{
    var meta = ViewData.Model as Clinic.Application.UI.UIMetadata;
    if (meta != null && meta.Mode == Clinic.Application.UI.RenderingMode.Metadata)
    {
        Layout = "_RegionLayout";
    }
    else
    {
        Layout = "_Layout";
    }
}
```

## How It Works
1. Every view in MVC inherently triggers `_ViewStart.cshtml` prior to compilation/rendering.
2. `_ViewStart.cshtml` inspects the strongly typed model (`ViewData.Model`).
3. If the model matches `UIMetadata` and specifies `RenderingMode.Metadata`, the framework injects `_RegionLayout.cshtml`.
4. If it specifies `RenderingMode.Template`, the framework bypasses `_RegionLayout` and uses the standard `_Layout.cshtml`, allowing the Template's native `RenderBody()` to evaluate.

This completely separates Template design from Framework routing logic.
