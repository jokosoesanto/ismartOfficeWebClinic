# 13 Screen Flow

Berdasarkan _Discovery Report_, berikut adalah alur navigasi antarmuka pengguna (Screen Flow) yang mengikat berbagai modul dalam Clinic Web. Flow ini menjabarkan perjalanan (*journey*) pengguna dari satu entitas ke entitas lainnya.

## 1. Patient Journey Flow
Alur utama terkait siklus hidup pasien di klinik:

```text
Dashboard
 ↓
Patient List (frmPatientManagement)
 ↓
Patient Detail (frmPatientHistory)
 ├─> Add/Edit Patient (frmAddEditPatient)
 ├─> Guardian Info (Tab/Modal)
 ├─> Patient Insurance Mapping (frmListInsurance)
 └─> Medical Alert (Context/Modal)
```

## 2. Clinical & Treatment Flow
Alur pemeriksaan medis setelah pasien tiba:

```text
Appointment / Office Console (frmOfficeConsole)
 ↓
Check-In Queue (RMOfficeCheckIn / Kiosk)
 ↓
Patient called to Chair
 ↓
Medical Record Dashboard
 ├─> Dental Chart / Odontogram (frmDentalChart)
 │    └─> AdultTeeth / ChildrenTeeth UI
 ├─> Digital Imaging & Radiography (frmDigitalImage)
 │    └─> Cephalometric Analysis (frmLateralDigitize)
 └─> Treatment Entry (Add Treatment Item)
```

## 3. Billing & Claim Flow
Alur pembayaran setelah perawatan medis selesai dilakukan:

```text
Treatment Entry
 ↓
Billing Generation
 ↓
Payment Entry (frmPayment)
 ├─> Print Receipt
 ├─> Dentist Statement (frmDentistStatement)
 └─> ADA / Canadian Claim Form (frmADAClaimForm)
```

## 4. Inventory Flow
Alur manajemen stok barang:

```text
Inventory List (frmInventoryList)
 ├─> Stock Item Master (frmStockItem)
 │    ├─> Suppliers
 │    ├─> Warehouses
 │    └─> Inventory Groups / Units
 └─> Add/Edit Transaction (frmAddEditTransaction)
      └─> Stock Mutation (In/Out)
```

## 5. Administration Flow
Alur pengaturan klinik:

```text
Admin Dashboard
 ├─> User List (frmUserList)
 │    └─> Role / Permission Management
 ├─> Location List (frmLocationList)
 │    └─> Chair Management
 ├─> Doctor Management (frmDoctorManagement)
 │    └─> Schedule / Working Hours (frmSchedule)
 ├─> License Configuration
 └─> Audit Logs
```
