# 19 UX Interaction Standard

Dokumen ini menjelaskan standar interaksi (*Interaction Design*) untuk menggantikan kebiasaan UX pada aplikasi Windows Desktop tradisional menuju Web UX yang asinkron.

## 1. Context Menus vs Action Buttons
- **Desktop**: Mengandalkan *Right-Click / Context Menu* pada baris grid (seperti di form `frmOfficeConsole`).
- **Web Standard**: Menggunakan ikon *Action* di kolom paling kanan dari tabel (biasanya berlabel "Actions"), atau menggunakan menu *Dropdown* (kebab icon / tiga titik vertikal `⋮`) untuk menghindari bentrok dengan klik kanan bawaan peramban (_browser native right-click_).

## 2. Windows Modal (ShowDialog) vs Web Modal
- **Desktop**: Form anak (Child Form) memblokir form induk.
- **Web Standard**: 
  - Gunakan **Bootstrap Modal** (`<clinic-modal>`) untuk tindakan singkat (konfirmasi hapus, input ringkas seperti "Add Chair").
  - Gunakan **Page Navigation** (pindah halaman) untuk input kompleks seperti pengisian rekam medis (Dental Chart) atau *Claim Form*.
  - *Nested Modals* (Modal di atas Modal) **DILARANG KERAS** dalam web UX. Ganti dengan *Wizard* langkah demi langkah.

## 3. Data Loading & Spinners
- **Desktop**: Aplikasi sering *freeze* (Not Responding) saat mengambil data SQL.
- **Web Standard**: Tombol _submit_ harus dinonaktifkan (disabled) dan digantikan oleh indikator _spinner_ seketika setelah ditekan. Tabel harus menampilkan state _Loading Skeleton_ atau _Spinner_ di area tengah.

## 4. Keyboard Shortcuts
- Pintasan papan ketik desktop kustom tidak direkomendasikan ditiru kecuali untuk Modul Kiosk (`RMOfficeCheckIn`) di mana input mungkin hanya dari alat _barcode scanner_ atau _numpad_ fisik.

## 5. Notification & Feedback
- Menggantikan *MessageBox.Show* dengan **Toast Notifications** (SweetAlert atau Bootstrap Toasts) yang muncul di sudut kanan atas layar tanpa menyita fokus (*non-blocking*). Pesan konfirmasi kritis (Delete Data) tetap menggunakan Modal konfirmasi.
