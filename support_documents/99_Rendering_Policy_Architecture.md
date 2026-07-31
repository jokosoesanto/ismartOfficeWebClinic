# Rendering Policy Architecture

## Overview
This document outlines the dual rendering architecture adopted by the iSmartOffice Web Clinic framework. The architecture supports two distinct rendering modes, explicitly managed by the framework and configured via `UIMetadata`.

## The Rendering Modes

### Mode 1: Metadata Rendering
- **Purpose**: Used for legacy prototype screens or components that rely entirely on Region Composition.
- **Workflow**: 
  `Controller` → `Metadata` → `Composition` → `ViewComponent` → `Browser`
- **Layout**: `_RegionLayout.cshtml`

### Mode 2: Template Rendering
- **Purpose**: Used for physical razor templates where the template acts as the main host of the page.
- **Workflow**:
  `Controller` → `Physical Razor Template` → `RenderBody()` → `Template` → `Widget ViewComponent (jika diperlukan)` → `Browser`
- **Layout**: `_Layout.cshtml` (bypassing region composition layout)

## Responsibilities

**Controller & Framework**
- Define the `RenderingMode`.
- Build the appropriate `UIMetadata`.
- Select the `Layout` globally based on the metadata mode.

**Razor Template**
- Focus entirely on HTML structure, UI, and UX.
- Invoke small `ViewComponents` locally as widgets.
- Contain zero knowledge of the underlying layout decision logic.
