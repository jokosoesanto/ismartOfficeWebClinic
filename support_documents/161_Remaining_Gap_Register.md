# Remaining Gap Register (Post-Freeze)

## Known & Accepted Gaps
The following items remain technically divergent from the Desktop application but are accepted as "By Design" for the Web platform or deferred to Backend Integration phases.

### 1. Keyboard Navigation Macros
- **Gap:** Desktop `F2`, `F3`, `Ctrl+S` macro shortcuts are not globally mapped in the browser prototype to avoid conflicts with native browser shortcuts.
- **Resolution:** Deferred to Post-MVP if requested by power users.

### 2. Multi-Window Capability
- **Gap:** Desktop allows opening 5 different patient charts simultaneously via MDI. The Web Prototype enforces a single active patient context per browser tab.
- **Resolution:** Web users can achieve the same result by Middle-Clicking (Open in New Tab) links. Native MDI is unsupported by modern web standards.

### 3. Settings / Master Data Depth
- **Gap:** Deep technical setups (e.g., Database Connection Strings, Printer Local Setup) present in Desktop Administration modules are obsolete in a Cloud-hosted Web application.
- **Resolution:** Omitted deliberately. Only business-level settings (Providers, Clinics) remain in the Admin module.

**Status:** The Prototype is considered 100% functionally complete from a Presentation perspective despite these accepted platform-specific gaps.
