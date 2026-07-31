# 78 Template Impact Assessment

## Analisis Dampak Global

Karena arsitektur UI kita dimotori oleh Metadata (dan setiap rute akan membacakan `TemplatePath` dari konfigurasi *Controller* masing-masing), ketiadaan beberapa file *Razor View Template* ini memiliki konsekuensi masif secara horizontal.

### Modul yang Terdampak (HTTP 500 / InvalidOperationException)
Modul-modul ini secara naif meminta template yang belum pernah dibuat oleh *developer*:
- **Dashboard** -> `Templates/Dashboard`
- **Appointment** -> `Templates/Scheduler`
- **Medical Record** -> `Templates/MedicalRecord`
- **Billing** -> `Templates/TransactionList`
- **Reports** -> `Templates/ReportViewer`

Semua rute di atas akan *CRASH* dan gagal menampilkan bahkan sekadar cangkang (shell) aplikasinya.

### Modul yang Aman (PASS / HTTP 200)
Modul-modul berikut selamat karena kebetulan dikonfigurasi untuk menggunakan `Templates/MasterList` dan `Templates/MasterDetail` (dua buah *template layout* yang secara fisik diciptakan pada fase *Foundation Sprint* awal):
- **Inventory** (`Templates/MasterList`)
- **Admin** (`Templates/MasterList`)
- **Patient** (`Templates/MasterList` & `Templates/MasterDetail`)

## Rekomendasi Solusi untuk Sprint Berikutnya
1. **Fix 1**: Ciptakan sisa *Razor View Templates* yang tertunda secara seragam di `Views/Shared/Templates/` (Dashboard, Scheduler, MedicalRecord, TransactionList, ReportViewer) dengan menyalin pola kerja `MasterList.cshtml`.
2. **Fix 2**: Modifikasi `MetadataFactory` atau infrastruktur terkait agar ketiadaan template fisik dapat dicegat sebelum *render pipeline* dilempar, mungkin dengan membuat satu *Generic Template Fallback* layaknya `Component Fallback` yang kita buat tempo hari. (Opsional, tergantung kebijakan tim).
