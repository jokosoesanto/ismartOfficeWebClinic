# Visual Regression Baseline

## Objective
Establish a visual standard (baseline) for all upcoming UI development, ensuring future features do not drift from the unified `clinic.ismartoffice.com` design tokens.

## Structural Baseline

### 1. The Global Frame
- **Sidebar Width**: 250px (expanded) / 60px (collapsed). Smooth transition applied (`0.3s ease`).
- **Header Height**: 60px fixed.
- **Background Context**: Body color is `#f9fafb`.
- **Text Defaults**: Base color is `#1f2937`. Font size maps cleanly from 14px (mobile/tablet) to 16px (desktop).

### 2. Form & Control State
- **Buttons**: All `.btn` elements enforce a `0.375rem` rounding. Primary actions use `#0073e6`.
- **Inputs**: `.form-control` elements share the `0.375rem` rounding. Focus states trigger a custom `#258cfb` outer ring.

### 3. Surface Elevations (Cards & Widgets)
- **Container Shape**: All `.card` panels enforce `0.625rem` (10px) border-radius to align with the softer web aesthetics of the iSmartOffice brand.
- **Drop Shadows**: Standardized `box-shadow: 0 1px 3px 0 rgba(0,0,0,0.1), 0 1px 2px -1px rgba(0,0,0,0.1)` provides slight elevation off the `#f9fafb` background.
- **Inner Padding**: Standard `.card-body` applies Bootstrap's standard spacing, often overridden by `p-3` or `p-4` structurally.

### 4. Data Display (Tables)
- **Structure**: Tables (`.table-responsive > .table`) fit strictly within their parents without overflowing into the sidebar.
- **Borders**: `.table-light` is used for table headers ensuring clear column distinction.

## Regression Triggers
Any future code commit that violates the structural baselines listed above (e.g. changing the sidebar toggling mechanism, removing `!important` tags from `site.css` component standardizations, or reintroducing the Theme Switcher UI) should trigger a Visual Regression Failure.
