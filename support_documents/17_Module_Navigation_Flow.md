# 17 Module Navigation Flow

## Inter-Module Linkages
Salah satu pilar aplikasi modern adalah kemudahan navigasi menyilang (*cross-navigation*). Berikut adalah _flow_ navigasi yang wajib dibangun pada prototipe:

1. **Dari `Dashboard`**:
   - Widget "Total Patients" -> Klik -> Ke `Patient List`.
   - Widget "Appt Today" -> Klik -> Ke `Appointment Console`.
   - Widget "Low Stock Alert" -> Klik -> Ke `Inventory List` (dengan filter *Low Stock*).

2. **Dari `Patient Detail`**:
   - Tab _Appointments_ -> Tombol [Book New] -> Ke `Appointment Calendar` dengan ID Pasien terisi otomatis.
   - Tombol Action [Medical Record] -> Membuka `Medical Record Dashboard` milik Pasien ini.
   - Tombol Action [New Payment] -> Membuka form `Payment` dengan data tagihan ter-inisialisasi.

3. **Dari `Appointment Console`**:
   - Baris Antrean Pasien (Row) -> Klik Kanan/Context Menu [View Patient] -> Membuka `Patient Detail`.
   - Context Menu [Start Treatment] -> Membuka `Dental Chart` pasien tersebut.

4. **Dari `Dental Chart` (Treatment)**:
   - Tombol [Submit to Billing] -> Mengarahkan ke halaman `Payment` / `Statement`.
   - Tombol [View X-Rays] -> Membuka `Digital Image` di Tab atau Modal baru tanpa menutup Odontogram.

5. **Dari `Inventory`**:
   - Daftar Stok -> Kolom _Supplier_ -> Tautan -> Membuka Modal `Supplier Detail`.

_Hyperlinking_ ini akan di-mock dengan me-lempar _dummy ID_ pada URL (_route segment_) contoh: `/MedicalRecord/Odontogram/1`.
