# Design Token Inventory
**Source**: `https://clinic.ismartoffice.com/`
**Target Environment**: iSmartOffice Web Clinic Prototype

## 1. Color Palette
Based on the extracted CSS styles and variables from the live application, the core color palette is mapped as follows:
- **Primary Color (Brand)**: `#0073e6` (iSmartOffice Blue, heavily used on the sidebar)
- **Secondary Color**: `var(--color-gray-500)` or equivalent Bootstrap `#6c757d`
- **Surface / Background Color**: `var(--color-gray-50)` (approx. `#f9fafb`) for main content.
- **Card Background**: `#ffffff`
- **Header / Topbar Background**: `#ffffff`
- **Sidebar Background**: `#0073e6` (Primary)
- **Sidebar Text**: `#ffffff`
- **Primary Text**: `var(--color-gray-800)` (approx `#1f2937`)
- **Border Color**: `var(--color-neutral-border)` / `CanvasText` (mapped to `#e5e7eb` or `#dee2e6` in Bootstrap context)

## 2. Typography
- **Base Font Size**: `14px` (scales to `16px` on min-width 768px screens).
- **Font Weight**: 
  - Standard text: `400`
  - Headings and active states: `600` (fw-bold)

## 3. Structural Properties
- **Border Radius**: `--radius: .625rem` (10px) - Gives cards and buttons a slightly softer, modern rounding compared to standard 4px.
- **Shadows**: 
  - Subdued shadows for cards: `0 1px 3px 0 rgba(0,0,0,0.1), 0 1px 2px -1px rgba(0,0,0,0.1)`
  - Elevated shadows for dropdowns/modals: `0 4px 6px -1px rgba(0,0,0,0.1)`
- **Grid Spacing / Padding**: Base `1rem` (16px), adjusted with standard responsive utilities.

## 4. Interactive States
- **Hover Transitions**: `.3s` default transition duration (`--tw-duration: .3s`).
- **Focus Rings**: Standard offset rings (`--tw-ring-color: #0798ff`).
- **Active Navigation State**: Represented by a slight background dimming on the blue sidebar or a high-contrast white active text color with a heavier font weight.

## Conclusion
This token inventory will serve as the exact mapping specification for the new unified `.root` CSS variables in `themes.css`. Assumptions regarding spacing, border-radius, and base font-sizes have been replaced by the actual live properties extracted from the host reference application.
