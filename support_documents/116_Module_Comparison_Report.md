# Module Comparison Report

## Dashboard Module
**Desktop Application (`frmMain` + Dashboard Panels)**:
- Heavily utilized for immediate notifications (Expiring items, Waitlist).
- Tightly coupled with the `Schedule` component.
- Lacks a true analytical dashboard; more of an operational quick-view.

**Web Prototype**:
- Dedicated `Dashboard.cshtml` template layout.
- Designed as a "Clinical Workspace" containing `Today's Schedule`, `Waiting Patients`, `Outstanding Cases`, and `Recent Patients`.
- **Verdict**: Web is functionally superior in design but lacks the live SignalR/WebSocket updates that the Desktop achieved natively.

## Patient Module
**Desktop Application (`frmPatient`, `frmPatientInfo`)**:
- Centralized `frmPatient` with extensive tabs (Demographics, Tx History, Odontogram, MedHx, Images).
- Hardcoded Sub-forms for editing (e.g. `frmPatientInfo`).

**Web Prototype**:
- Replicates layout via `_RegionLayout` containing `PatientSummary` (North) and `PatientTabs` (Center).
- Uses ViewComponents to lazy-load tab data.
- **Verdict**: PARTIAL. Navigation and architecture match, but advanced charting components (Odontogram) and DICOM/Image viewers are entirely missing from the Web prototype.

## Appointment / Scheduler Module
**Desktop Application (`frmSchedule`)**:
- Highly interactive timeline UI capable of drag-and-drop.
- Right-click context menus for changing status (Arrived, In-Chair, Completed).

**Web Prototype**:
- Scaffolding exists, layout assigned (`Scheduler.cshtml`).
- **Verdict**: MISSING. Drag-and-drop capability and context menus must be mapped to a complex JS component (e.g., FullCalendar) in future sprints.

## Administration & Configuration
**Desktop Application (`frmDatabaseSetup`, `frmSettings`)**:
- Contains IT-level configurations (Local DB path, Server IP, File Paths).
- Contains Master Data (Locations, Chairs, Insurance).

**Web Prototype**:
- Master Data exists under Admin (Users, Roles, Locations, Chairs).
- DB/Server config correctly deprecated (moved to backend config).
- **Verdict**: DIFFERENT. Web rightly drops IT configurations, but must implement the missing Master Data lists (Insurance, Doctors, Procedures).
