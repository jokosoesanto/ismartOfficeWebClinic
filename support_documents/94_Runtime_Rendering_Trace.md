# Runtime Rendering Trace

This document captures the exact runtime path taken by each main module request, proving objectively why the new templates are not visible.

## 1. Dashboard
* **Request URL:** `GET /`
* **Controller:** `HomeController`
* **Action:** `Index`
* **Metadata:** `Center = [PrototypeDashboard]`
* **Template Name:** `Templates/Dashboard`
* **Physical Razor File:** `Views/Shared/Templates/Dashboard.cshtml` (Executed)
* **ViewComponent (Layout Override):** `PrototypeDashboard`
* **Rendered HTML:** Contents of `PrototypeDashboard`, overriding the template.

## 2. Patient
* **Request URL:** `GET /Patient`
* **Controller:** `PatientController`
* **Action:** `Index`
* **Metadata:** `Center = [DataGrid (title="Registered Patients")]`
* **Template Name:** `Templates/Patient_List`
* **Physical Razor File:** `Views/Shared/Templates/Patient_List.cshtml` (Executed)
* **ViewComponent (Layout Override):** `DataGrid`
* **Rendered HTML:** Contents of `DataGrid`, overriding the template.

## 3. Appointment
* **Request URL:** `GET /Appointment`
* **Controller:** `AppointmentController`
* **Action:** `Index`
* **Metadata:** `Center = [PrototypeScheduler]`
* **Template Name:** `Templates/Scheduler`
* **Physical Razor File:** `Views/Shared/Templates/Scheduler.cshtml` (Executed)
* **ViewComponent (Layout Override):** `PrototypeScheduler`
* **Rendered HTML:** Contents of `PrototypeScheduler`, overriding the template.

## 4. Billing
* **Request URL:** `GET /Billing`
* **Controller:** `BillingController`
* **Action:** `Index`
* **Metadata:** `Center = [PrototypeTransaction]`
* **Template Name:** `Templates/Payment_History`
* **Physical Razor File:** `Views/Shared/Templates/Payment_History.cshtml` (Executed)
* **ViewComponent (Layout Override):** `PrototypeTransaction`
* **Rendered HTML:** Contents of `PrototypeTransaction`, overriding the template.

## 5. Inventory
* **Request URL:** `GET /Inventory`
* **Controller:** `InventoryController`
* **Action:** `Index`
* **Metadata:** `Center = [PrototypeMasterDetail]`
* **Template Name:** `Templates/Inventory_List`
* **Physical Razor File:** `Views/Shared/Templates/Inventory_List.cshtml` (Executed)
* **ViewComponent (Layout Override):** `PrototypeMasterDetail`
* **Rendered HTML:** Contents of `PrototypeMasterDetail`, overriding the template.

## 6. Medical Record
* **Request URL:** `GET /MedicalRecord`
* **Controller:** `MedicalRecordController`
* **Action:** `Index`
* **Metadata:** `Center = [PrototypeTransaction]`
* **Template Name:** `Templates/MR_Chart`
* **Physical Razor File:** `Views/Shared/Templates/MR_Chart.cshtml` (Executed)
* **ViewComponent (Layout Override):** `PrototypeTransaction`
* **Rendered HTML:** Contents of `PrototypeTransaction`, overriding the template.

## 7. Report
* **Request URL:** `GET /Report`
* **Controller:** `ReportController`
* **Action:** `Index`
* **Metadata:** `Center = [PrototypeReport]`
* **Template Name:** `Templates/ReportViewer`
* **Physical Razor File:** `Views/Shared/Templates/ReportViewer.cshtml` (Executed)
* **ViewComponent (Layout Override):** `PrototypeReport`
* **Rendered HTML:** Contents of `PrototypeReport`, overriding the template.

## 8. Admin
* **Request URL:** `GET /Admin`
* **Controller:** `AdminController`
* **Action:** `Index`
* **Metadata:** `Center = [PrototypeAdministration]`
* **Template Name:** `Templates/Admin_List`
* **Physical Razor File:** `Views/Shared/Templates/Admin_List.cshtml` (Executed)
* **ViewComponent (Layout Override):** `PrototypeAdministration`
* **Rendered HTML:** Contents of `PrototypeAdministration`, overriding the template.
