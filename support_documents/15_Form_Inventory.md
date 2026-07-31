# 15 Form Inventory

Berikut adalah inventaris seluruh form aplikasi Desktop dari _Discovery Report_ yang akan dipetakan menjadi Screen pada Web Prototype.

| Original Form Name | Web Route / Module | Purpose | Action Buttons / UI Elements |
| ------------------ | ------------------ | ------- | ---------------------------- |
| **`frmPatientManagement`** | `/Patient` | Menampilkan daftar pasien (Grid). | Search, Filter, Add, Edit, Delete, Export |
| **`frmAddEditPatient`** | `/Patient/Create`, `/Patient/Edit/{id}` | Form input data demografi pasien. | Tabs (Demographics, Guardian, Insurance, Alert), Save, Cancel |
| **`frmPatientHistory`** | `/Patient/Details/{id}` | Melihat rincian pasien secara lengkap. | Edit, Back to List, View Medical Record |
| **`frmOfficeConsole`** | `/Appointment/Console` | Manajemen antrean (*Queue*) harian klinik. | DataGrid, ContextMenu (Call Patient, Check-out), Auto-refresh |
| **`frmAppointment`** | `/Appointment/Calendar` | Penjadwalan pertemuan dengan kalender visual. | Drag & Drop Events, View Day/Week/Month, Create Appt |
| **`RMOfficeCheckIn`** | `/Kiosk/CheckIn` | Kiosk mandiri untuk konfirmasi kedatangan pasien. | Touch UI, Numpad, Status indicator |
| **`frmDoctorManagement`**| `/Doctor` | Kelola master data Dokter. | Grid, Add, Edit, Set Schedule |
| **`frmSchedule`** | `/Doctor/Schedule/{id}` | Menentukan jam kerja (roster) dokter. | Timeline grid, Save, Copy from last week |
| **`frmDentalChart`** | `/MedicalRecord/Odontogram/{id}`| Perekaman kondisi gigi visual (Adult/Child). | Toolbar (Caries, Filling, Missing), Canvas, Save |
| **`frmDigitalImage`**| `/MedicalRecord/Images/{id}` | Manajemen foto X-Ray/Radiografi pasien. | Image Viewer, Zoom, Rotate, Analyze |
| **`frmLateralDigitize`**| `/MedicalRecord/Cephalometric`| Analisis titik sefalometri. | Canvas markers, Calculation Panel |
| **`frmPayment`** | `/Billing/Payment` | Transaksi pembayaran kasir. | Grid Items, Payment Method Select, Pay, Print |
| **`frmDentistStatement`**| `/Billing/Statement` | Laporan/tagihan dokter ke pasien. | Date Filter, Print |
| **`frmADAClaimForm`** | `/Claim/ADA` | Pengajuan klaim asuransi ADA form (US). | Complex PDF Form Renderer, Submit, Print |
| **`frmInventoryList`**| `/Inventory` | Tabel daftar stok item klinik. | Grid, Add, Restock, Edit |
| **`frmStockItem`** | `/Inventory/Item` | Form pembuatan/update item inventori. | Master Lookup (Supplier, Unit), Save |
| **`frmAddEditTransaction`**| `/Inventory/Transaction` | Input mutasi stok barang masuk/keluar. | Dropdown Type (In/Out), Quantity input, Save |
| **`frmListInsurance`**| `/Admin/Insurance` | Master perusahaan asuransi. | Grid, Add, Edit, Delete |
| **`frmUserList`** | `/Admin/Users` | Daftar pengguna aplikasi dan role-nya. | Grid, Reset Password, Disable, Edit Permissions |
| **`frmLocationList`** | `/Admin/Locations` | Daftar cabang klinik. | Grid, Edit, Manage Chairs |
| **`frmCustomReport`** | `/Report/Custom` | Pembangun kueri laporan dinamis. | Drag-drop fields, Run Query, Export Excel |
