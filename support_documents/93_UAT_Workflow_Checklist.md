# User Acceptance Testing (UAT) Workflow Checklist

## Goal
Verify that the Web Prototype satisfies the workflow parity requirements with the Desktop Application, focusing on Navigation, Screen Completeness, and Visual Layout.

## 1. Patient Registration Workflow
- [ ] Open Application -> Land on Dashboard.
- [ ] Click `Patient` from Sidebar -> Verify `Patient_List` opens.
- [ ] Verify Search bar and Table structure matches Desktop `frmPatientManagement`.
- [ ] Click `Add Patient` -> Verify `Patient_Form` opens.
- [ ] Verify General Info, Insurance, and Medical Alert tabs exist.
- [ ] From `Patient_List`, click `Details` -> Verify `Patient_Detail` opens with correct panels.

## 2. Scheduling Workflow
- [ ] Click `Appointment` from Sidebar -> Verify `Scheduler` opens.
- [ ] Verify Calendar grid is present with Provider columns (Dr. Alan, Dr. Emily Chen).
- [ ] Click `Add Schedule` -> Verify `Scheduler_Form` opens.
- [ ] Verify fields: Patient Lookup, Provider, Date, Time, Reason.
- [ ] From `Scheduler`, click `Detail` -> Verify `Scheduler_Detail` opens.

## 3. Clinical Charting Workflow
- [ ] Navigate to `Medical Record` -> Verify `MR_Chart` opens.
- [ ] Verify Odontogram placeholder and right-side panels (Findings, Treatments) are present.
- [ ] Click `Add Treatment` -> Verify `MR_TreatmentForm` opens.
- [ ] Verify tooth number, surface, procedure dropdowns exist.
- [ ] Click `Treatment History` -> Verify `MR_History` opens with past procedures.

## 4. Billing Workflow
- [ ] Navigate to `Billing` -> Verify `Payment_History` opens.
- [ ] Click `New Payment` -> Verify `Payment_Form` opens.
- [ ] Verify layout parity with Desktop `frmPayment` (Appointments on left, Payment Method on right).
- [ ] Click `Make Payment` -> Verify `Payment_Preview` (Receipt) opens.

## 5. Inventory Workflow
- [ ] Navigate to `Inventory` -> Verify `Inventory_List` opens.
- [ ] Verify TreeView on the left and DataGrid on the right.
- [ ] Click `Add Item` -> Verify `Inventory_ItemForm` opens.
- [ ] Click `Details` on an item -> Verify `Inventory_Detail` shows Low Stock alerts and movement history.

## 6. Report Workflow
- [ ] Navigate to `Report` -> Verify `ReportViewer` opens.
- [ ] Verify Report Categories, Filters, and A4 Preview are on the same page.

## 7. Admin Workflow
- [ ] Navigate to `Admin` -> Verify `Admin_List` opens.
- [ ] Click `Add User` -> Verify `Admin_UserForm` opens.
- [ ] Click `Details` on a User -> Verify `Admin_UserDetail` shows Effective Permissions.
