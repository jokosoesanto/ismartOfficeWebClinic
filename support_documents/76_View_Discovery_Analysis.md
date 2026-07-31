# 76 View Discovery Analysis

## Engine Search Path
Berdasarkan log *Exception*, *Razor View Engine* mencari *View* secara berurutan pada dua lokasi konvensional:
1. `/Views/Home/Templates/Dashboard.cshtml` (Folder *Controller Specific*)
2. `/Views/Shared/Templates/Dashboard.cshtml` (Folder *Shared / Fallback*)

## Physical File Availability
Daftar seluruh file yang ada di dalam *Shared Templates* saat ini:
- `Views/Shared/Templates/MasterDetail.cshtml` (Eksis)
- `Views/Shared/Templates/MasterList.cshtml` (Eksis)

## Gap Analysis (Expected vs Actual)
- **Expected**: Framework memiliki file `.cshtml` untuk seluruh jenis arsitektur metadata (Dashboard, Scheduler, MedicalRecord, TransactionList, ReportViewer, dll).
- **Actual**: Saat *Foundation UI Sprint*, arsitek *front-end* hanya sempat membangun `MasterList` dan `MasterDetail`. Template lainnya tertinggal secara konseptual dan tidak pernah diwujudkan sebagai file `.cshtml` fisik.
