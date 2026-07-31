# Template Usage Matrix

This document maps all the new templates created during the previous sprint to their actual runtime usage.

| Template Name | File exists? | Pernah dirender? | Siapa yang memanggil? | URL apa? | Bukti HTML hasil render |
|---|---|---|---|---|---|
| `Patient_List` | Yes | Yes (but ignored) | `PatientController.Index` | `GET /Patient` | Replaced by `DataGrid` ViewComponent HTML |
| `Patient_Form` | Yes | Yes (but ignored) | `PatientController.Create`, `.Edit` | `GET /Patient/Create` | Replaced by `PatientForm` ViewComponent HTML |
| `Patient_Detail` | Yes | Yes (but ignored) | `PatientController.Details` | `GET /Patient/{id}` | Replaced by `PatientTabs` ViewComponent HTML |
| `Scheduler` | Yes | Yes (but ignored) | `AppointmentController.Index` | `GET /Appointment` | Replaced by `PrototypeScheduler` HTML |
| `Scheduler_Form` | Yes | Yes (but ignored) | `AppointmentController.Create`, `.Edit` | `GET /Appointment/Create` | Replaced by `PrototypeScheduler` HTML |
| `Scheduler_Detail` | Yes | Yes (but ignored) | `AppointmentController.Details` | `GET /Appointment/{id}` | Replaced by `PrototypeScheduler` HTML |
| `Payment_History` | Yes | Yes (but ignored) | `BillingController.Index` | `GET /Billing` | Replaced by `PrototypeTransaction` HTML |
| `Payment_Form` | Yes | Yes (but ignored) | `BillingController.Create` | `GET /Billing/Create` | Replaced by `PrototypeTransaction` HTML |
| `Payment_Preview` | Yes | Yes (but ignored) | `BillingController.Preview` | `GET /Billing/Preview/{id}` | Replaced by `PrototypeTransaction` HTML |
| `Inventory_List` | Yes | Yes (but ignored) | `InventoryController.Index` | `GET /Inventory` | Replaced by `PrototypeMasterDetail` HTML |
| `Inventory_Detail` | Yes | Yes (but ignored) | `InventoryController.Details` | `GET /Inventory/{id}` | Replaced by `PrototypeMasterDetail` HTML |
| `Inventory_ItemForm` | Yes | Yes (but ignored) | `InventoryController.CreateItem` | `GET /Inventory/CreateItem` | Replaced by `PrototypeMasterDetail` HTML |
| `Inventory_GroupForm`| Yes | Yes (but ignored) | `InventoryController.CreateGroup`| `GET /Inventory/CreateGroup` | Replaced by `PrototypeMasterDetail` HTML |
| `MR_Chart` | Yes | Yes (but ignored) | `MedicalRecordController.Index` | `GET /MedicalRecord` | Replaced by `PrototypeTransaction` HTML |
| `MR_History` | Yes | Yes (but ignored) | `MedicalRecordController.History` | `GET /MedicalRecord/History` | Replaced by `PrototypeTransaction` HTML |
| `MR_Detail` | Yes | Yes (but ignored) | `MedicalRecordController.Details` | `GET /MedicalRecord/{id}` | Replaced by `PrototypeTransaction` HTML |
| `MR_TreatmentForm` | Yes | Yes (but ignored) | `MedicalRecordController.CreateTreatment` | `GET /MedicalRecord/CreateTreatment` | Replaced by `PrototypeTransaction` HTML |
| `ReportViewer` | Yes | Yes (but ignored) | `ReportController.Index` | `GET /Report` | Replaced by `PrototypeReport` HTML |
| `Admin_List` | Yes | Yes (but ignored) | `AdminController.Index` | `GET /Admin` | Replaced by `PrototypeAdministration` HTML |
| `Admin_UserForm` | Yes | Yes (but ignored) | `AdminController.CreateUser` | `GET /Admin/CreateUser` | Replaced by `PrototypeAdministration` HTML |
| `Admin_UserDetail` | Yes | Yes (but ignored) | `AdminController.UserDetails` | `GET /Admin/UserDetails/{id}`| Replaced by `PrototypeAdministration` HTML |

## Conclusion
* **All templates exist physically.**
* **All templates are correctly returned by the Controller.**
* **All templates are executed by Razor.**
* **NONE of the templates ever make it to the browser.** The HTML they produce is discarded because a ViewComponent overrides it.
