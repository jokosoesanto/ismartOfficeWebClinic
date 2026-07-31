# Patient Module UAT (User Acceptance Testing)

## Capability: Patient Complete Parity

### Scenario 1: Verify New Patient Demographics
**Steps:**
1. Navigate to `Web Prototype -> Patient -> Add Patient`.
2. Observe the General Info form.
3. **Verify** the `SSN / KTP` text box is present and marked with a red `*` (Required).
4. **Verify** the `Blood Type` dropdown contains standard options (A+, O-, etc).
5. **Verify** the `Occupation` and `Employer` fields exist.
6. **Verify** the `Active Patient` toggle switch is present, replacing the legacy checkbox.
7. Click the **Upload Photo** button. Select a `.jpg` or `.png` file.
8. **Verify** the generic person icon disappears and the uploaded photo is previewed dynamically inside the circular frame.

### Scenario 2: Verify Patient Detail Display
**Steps:**
1. Navigate to `Web Prototype -> Patient -> List`.
2. Click the **Eye (Details)** icon on the first patient.
3. **Verify** the `#PT-2601001` text now includes a green `Active` badge next to it.
4. **Verify** the list below the circular avatar now explicitly shows the `SSN/KTP`, `Blood Type`, and `Occupation` injected with matching Bootstrap Icons (Credit Card, Blood Droplet, Briefcase).

### Scenario 3: Verify Data Deletion Protection Rule
**Steps:**
1. Navigate to `Web Prototype -> Patient -> List`.
2. Locate the first patient ("John Smith"). Click the red **Trash (Delete)** icon.
3. A Modal should pop up.
4. **Verify** the Modal title says "Delete John Smith?".
5. **Verify** the Modal shows a yellow Warning alert: *"Deletion Blocked... This patient cannot be deleted because they have existing Medical Records"*.
6. **Verify** the red "Delete Patient" button is disabled (unclickable).
7. Close the Modal. Locate the second patient ("Jane Smith"). Click the red **Trash (Delete)** icon.
8. **Verify** the Modal shows "Delete Jane Smith?".
9. **Verify** the Warning alert is hidden, and the grey text says "Are you sure you want to delete this patient record?".
10. **Verify** the red "Delete Patient" button is active (clickable).
