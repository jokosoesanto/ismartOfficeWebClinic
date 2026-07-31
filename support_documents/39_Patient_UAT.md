# 39 Patient Manual UAT Scenarios

## Tujuan UAT
Mengonfirmasi keberhasilan implementasi UI Composition Engine dan Component Registry untuk Modul Patient.

## Skenario UAT

### 1. Verifikasi Routing & Navigation
- Buka rute `/Patient`.
- **Ekspektasi**: Muncul DataGrid _Registered Patients_ dengan data _Mock_. Layout menggunakan _Center Region_.
- Buka rute `/Patient/Details/P001`.
- **Ekspektasi**: Muncul halaman Detail. Title di Header adalah "Patient Details - P001".

### 2. Verifikasi Region Layout & Composition
Pada Halaman Detail (`/Patient/Details/P001`):
- Periksa area **Utara (North)**.
  - **Ekspektasi**: Harus ada kotak profil (Summary) John Doe, dengan tombol [Book Appt].
- Periksa area **Timur (East)**.
  - **Ekspektasi**: Harus ada panel "Medical Alerts" warna peringatan merah/kuning (Alergi: Penicillin).
- Periksa area **Tengah (Center)**.
  - **Ekspektasi**: Harus ada kontrol tabulasi (*Demographics*, *Guardian*, *Insurance*, *Tx History*).

### 3. Verifikasi UI Component States (Tab & Lazy Component)
- Klik tab **Guardian**.
  - **Ekspektasi**: Menampilkan form Guardian (Mary Doe).
- Klik tab **Insurance**.
  - **Ekspektasi**: Menampilkan tabel asuransi (BlueCross BlueShield).
- Klik tab **Tx History**.
  - **Ekspektasi**: Menampilkan indikator loading _Spinner_ dengan keterangan "Loading treatment history...". Ini membuktikan _Lazy Component placeholder_ berfungsi sebelum di-*hydrate* oleh JS.

### 4. Verifikasi Theme Switching (Regression)
- Gunakan Theme Switcher di navigasi utama, ganti tema dari _Medical Blue_ ke _Dark Mode_.
- **Ekspektasi**: Latar belakang `PatientSummary` dan `MedicalAlert` berubah menjadi nuansa gelap mengikuti CSS Variables. Komponen tidak hancur.
