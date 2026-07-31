# Risk Assessment Report

## 1. Missing Business Risk
**Risk Level: CRITICAL**
The Desktop Application utilizes an interactive graphic `Odontogram` for visual tooth charting, which inherently populates the `Tx History` (Treatment History) grid. The Web Prototype currently only models the Grid. Without the Odontogram, clinical users cannot efficiently chart existing conditions and proposed treatments, causing a complete operational failure for dental practitioners.
- **Mitigation**: Prioritize the evaluation of SVG/Canvas based Odontogram components (Sprint 4) to ensure technical feasibility early on.

## 2. UX Risk
**Risk Level: HIGH**
The Desktop Application (`frmSchedule`) uses an extremely responsive, drag-and-drop Timeline control for Appointment booking, paired with right-click context menus. Web users generally expect slightly slower interactions, but failing to provide a drag-and-drop Javascript calendar will result in a severe UX downgrade compared to the legacy system.
- **Mitigation**: Implement `FullCalendar.io` or a similar robust Web Component in Sprint 3.

## 3. Migration Risk
**Risk Level: MEDIUM**
The legacy application allowed multiple MDI windows (Multiple Document Interface), enabling a user to have 3 patients open simultaneously. The Web App enforces a single "Active Patient" state governed by the Top Header.
- **Mitigation**: The Product Owner must formally sign off on this UX paradigm shift. End-user training will be required to teach users to use "Browser Tabs" rather than "Application Windows".

## 4. User Adoption Risk
**Risk Level: LOW**
While shortcuts (F2, F3) and global toolbars are currently missing in the Web prototype, the navigation structure (Sidebar) has been meticulously mapped to the legacy Module tree. The familiar nomenclature will ease adoption.
- **Mitigation**: Implement the Global "Quick Add" toolbar in the Web Header to compensate for the loss of the Desktop Ribbon.

## 5. Technical Risk
**Risk Level: MEDIUM**
The `frmCustomReport` module in Desktop provided dynamic column generation and ad-hoc filtering. Replicating this generic Query Builder purely in the Web Presentation Layer without overloading the database is technically demanding.
- **Mitigation**: Rely on a robust backend API returning serialized JSON configurations, coupled with a frontend Grid component like DataTables or AG-Grid to handle dynamic rendering locally.
