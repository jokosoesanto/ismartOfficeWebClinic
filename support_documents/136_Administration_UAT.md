# Administration & Master Data UAT

## Phase 2 Capability UAT

### Scenario 1: Administration Navigation
**Steps:**
1. Navigate to the Prototype (`/Admin/Users`).
2. **Verify** the Sidebar contains an `Administration` section with: `User Accounts`, `Roles & Permissions`, `Configuration`.
3. **Verify** the Sidebar contains a `Master Data` section with: `Doctors / Providers`, `Insurance`, `Procedures`, `Locations`, `Chairs / Rooms`, `Master Lookup`.
4. Click through each link.
5. **Verify** the Page Title updates automatically.
6. **Verify** the Data Grid headers change to match the entity (e.g. `Manage Doctors` shows `Provider ID`, `Specialty`).

### Scenario 2: Master Data CRUD Dialog Prototype
**Steps:**
1. While on `/Admin/Procedures`, click the blue `Add Record` button in the top right.
2. **Verify** the "Add/Edit Record" modal appears.
3. Leave all fields blank and click "Save Changes".
4. **Verify** the browser's native HTML5 validation blocks the submission and highlights the missing fields (Primary Identifier, Description).
5. Fill in the fields with dummy data ("001", "Extraction").
6. Click "Save Changes".
7. **Verify** a JavaScript alert appears: "Record saved successfully! (Simulated)" and the modal automatically closes.
8. Locate a row in the data grid and click the Pencil (Edit) icon.
9. **Verify** the same Add/Edit modal appears for modification.

### Scenario 3: Tools Parity
**Steps:**
1. **Verify** the Search bar is present above the data grid.
2. **Verify** the layout is fully responsive and matches the rest of the application's Design System.
