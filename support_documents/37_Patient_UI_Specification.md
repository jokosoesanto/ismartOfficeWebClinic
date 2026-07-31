# 37 Patient UI Specification

Dokumen ini menjabarkan spesifikasi _Vertical Slice_ pertama: Modul **Patient**.

## Route Binding
Seluruh akses ke Modul Pasien menggunakan _Route_ final:
- `/Patient` : Membuka Halaman Daftar Pasien (List).
- `/Patient/Details/{id}` : Membuka Halaman Rekam Singkat Pasien (Detail).
- `/Patient/Create` : Membuka Halaman Tambah Pasien.
- `/Patient/Edit/{id}` : Membuka Halaman Edit Pasien.

## Screen Composition

### Patient List (`/Patient`)
- **Template**: `MasterList.cshtml` (menggunakan `_RegionLayout.cshtml`).
- **Composition**:
  - `Center Region`: `DataGrid` Component (Daftar Pasien Mock).

### Patient Detail (`/Patient/Details/{id}`)
- **Template**: `MasterDetail.cshtml`.
- **Composition**:
  - `North Region`: `PatientSummary` Component (Foto, Nama, Tombol Aksi Cepat).
  - `Center Region`: `PatientTabs` Component (Demografi, Guardian, Insurance, Lazy History).
  - `East Region`: `MedicalAlert` Component (Alergi, Penyakit bawaan).

*Note: Seluruh antarmuka ini 100% didorong oleh UI Metadata Object di `PatientController.cs` yang memanggil `ComponentRegistry`, tanpa ada view HTML kustom spesifik per halaman (No Monolithic View).*
