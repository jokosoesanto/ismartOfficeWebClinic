# 70 Prototype Component Catalog

Berikut adalah katalog komponen prototipe yang saat ini menopang seluruh menu aplikasi di tahap pra-implementasi *business logic*:

## 1. PrototypeDashboard
- **Ikon**: Speedometer
- **Karakteristik**: Ditujukan untuk halaman muka utama.
- **Dipakai Oleh**: `HomeController` (rute `/`)

## 2. PrototypeList
- **Ikon**: List UL
- **Karakteristik**: Menampilkan tabel dengan toolbar pencarian dan aksi "Add New".
- **Dipakai Oleh**: `AppointmentController`, `InventoryController`

## 3. PrototypeMasterDetail
- **Ikon**: Layout Split
- **Karakteristik**: Digunakan untuk layar yang kompleks dengan area detail atau rekam medis.
- **Dipakai Oleh**: `MedicalRecordController`

## 4. PrototypeTransaction
- **Ikon**: Receipt
- **Karakteristik**: Memuat format transaksi finansial.
- **Dipakai Oleh**: `BillingController`

## 5. PrototypeReport
- **Ikon**: File Earmark Bar Graph
- **Karakteristik**: Simulasi laporan dengan parameter dan grafik/tabel hasil.
- **Dipakai Oleh**: `ReportController`

## 6. PrototypeAdministration
- **Ikon**: Gear
- **Karakteristik**: Digunakan secara massal di berbagai sub-menu Admin.
- **Dipakai Oleh**: `AdminController` (rute `/Admin`, `/Admin/Users`, `/Admin/Roles`, `/Admin/Locations`, `/Admin/Chairs`)

Setiap komponen ini memiliki badge (watermark) bernada kuning bertuliskan **Prototype 0.1** yang mencegah *stakeholder* mengiranya sebagai modul jadi.
