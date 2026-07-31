# Sidebar Visual Hotfix Report

## Bug Description
During the UAT of the Design Convergence Sprint, it was discovered that the textual items on the Sidebar (Navigation Links, Brand Title) were practically invisible under the `medical-blue` theme. The `medical-blue` theme enforced `#0073e6` (Blue) as the sidebar background, while the child elements were rendering using dark Bootstrap utility classes (`.text-main`, `.text-muted`, `.text-primary`), causing a severe WCAG contrast violation.

## Root Cause
- The sidebar's inner template (`Views/Shared/Components/Sidebar/Default.cshtml`) utilized static generic typography utilities that were optimized for a white background context.
- When the Single Source of Truth required the sidebar to be a primary blue color, the static text color utilities forced dark grey `#1f2937` and blue `#0073e6` text against the blue background `#0073e6`.

## Resolution Strategy
1. **Dynamic Theme Context**: A new suite of Design Tokens was established inside the `themes.css` Engine (`--sidebar-text`, `--sidebar-text-muted`, `--sidebar-hover-bg`, `--sidebar-active-bg`).
2. **CSS Specificity Override**: Rather than fundamentally rewriting the C# ViewComponent and stripping Bootstrap classes, the presentation layer was targeted in `themes.css`. By explicitly applying `color: var(--sidebar-text) !important;` to `.sidebar .text-main`, `.text-primary`, and `.text-muted`, the generic classes were intercepted and recolored dynamically based on the active theme.
3. **State Management Integration**: Hover and Active states were similarly decoupled from generic Bootstrap defaults and wired into the specialized `--sidebar-hover-bg` (`rgba(255, 255, 255, 0.1)`) and `--sidebar-active-bg` (`rgba(255, 255, 255, 0.2)`) tokens.

## Verification
- **Default State**: Text is crisp white on blue (High Contrast).
- **Hover State**: Highlights subtly without affecting text contrast.
- **Active State**: Navigation link is distinct (solid background, `fw-bold` white text).
- **Collapsed/Expanded**: Icons maintain perfect contrast and visibility across transitions.
