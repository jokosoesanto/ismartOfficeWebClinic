# Field Level Parity Report

## 1. Patient Demographics (frmPatientInfo vs Patient ViewComponent)
**Missing Fields in Web Prototype:**
- `txtSSN` (Social Security Number / KTP) - Missing entirely.
- `cboBloodType` - Missing.
- `chkIsActive` - The status toggle is missing from the Web form.
- `txtEmployer` & `txtOccupation` - Missing.
- `picPatient` (Photo upload) - Placeholder exists, but upload field mechanism is missing.

**Different Fields:**
- `txtDOB` in Desktop is a standard masked text box; Web Prototype utilizes an HTML5 `<input type="date">` which is superior but changes UX behavior.
- `cboGender` is a dropdown in Desktop, currently mocked as a string display in Web. Needs a `<select>` implementation.

## 2. Appointment Scheduling (frmSchedule vs Appointment ViewComponent)
**Missing Fields in Web Prototype:**
- `cboChair` / `cboRoom` filter. (Desktop allows viewing schedules per specific dental chair).
- `chkShowCancelled` - Toggle to show/hide cancelled appointments is missing.
- `lblStatusColorCode` - Legend mapping is missing.

## 3. Custom Reporting (frmCustomReport vs Web Report Module)
**Missing Fields in Web Prototype:**
- The Desktop contains complex Field Pickers (`lvColumnView`, `lvColumnFilter`) for dynamically generating report columns.
- `btnAddFilter`, `cboListView` are completely absent in the Web Prototype which currently mocks static reports.

## 4. Default Values & Validations
- **DOB Validation**: Desktop prevents future dates dynamically. Web relies on generic HTML5 behavior.
- **Mandatory Fields**: Desktop enforced `LastName` and `Phone`. Web prototype currently has no client-side validation (`jquery.validate` not yet implemented on forms).
- **Lookup Types**: Desktop uses bound `ComboBoxes` tightly coupled to `DataSet`. Web prototype requires AJAX lookup endpoints for dropdowns (e.g., searching for a Doctor in the Appointment form).
