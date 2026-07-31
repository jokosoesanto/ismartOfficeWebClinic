# Sidebar Icon Audit

## Audit Parameters
This document audits the usage of Bootstrap Icons across the primary Navigation Sidebar for iSmartOffice Web Clinic. The primary constraint evaluated is whether the icon correctly communicates the business function and ensures no fallback placeholder icons (e.g. `-`, `*`, `>`) remain in the UI.

## Parent Menus
| Menu Group | Icon Used | Relevance |
|------------|-----------|-----------|
| **Dashboard** | `bi-speedometer2` | Universally represents performance/metrics. |
| **Patient** | `bi-person-lines-fill` | Represents patient registry/records. |
| **Appointment** | `bi-calendar-check` | Scheduling and confirmed bookings. |
| **Medical Record** | `bi-clipboard2-pulse` | Clinical / Medical charting history. |
| **Billing & Payment** | `bi-cash-coin` | Financial transactions. |
| **Inventory** | `bi-box-seam` | Physical product and stock levels. |
| **Reports** | `bi-bar-chart` | Data visualization. |
| **Administration** | `bi-gear` | Core system configuration. |

## Submenus (Replaced Placeholders)
| Parent Group | Submenu Title | Replaced Icon | New Professional Icon | Rationale |
|--------------|---------------|---------------|-----------------------|-----------|
| **Patient** | All Patients | `bi-dash (-)` | `bi-people` | Indicates a collective list or registry of multiple patients. |
| **Patient** | Add Patient | `bi-dash (-)` | `bi-person-plus` | Explicit "Add" symbol attached to a person. |
| **Administration** | Users | `bi-dash (-)` | `bi-person` | Individual user account configuration. |
| **Administration** | Roles | `bi-dash (-)` | `bi-shield-lock` | Security permissions and role-based access. |
| **Administration** | Locations | `bi-dash (-)` | `bi-buildings` | Represents physical clinic branches or facilities. |
| **Administration** | Chairs | `bi-dash (-)` | `bi-display` | Represents physical operational workstations (dental/medical chairs). |

## Conclusion
The audit confirms 100% compliance. All placeholders have been eliminated and visually replaced with high-quality, contextual Bootstrap Icons matching the established iSmartOffice web identity.
