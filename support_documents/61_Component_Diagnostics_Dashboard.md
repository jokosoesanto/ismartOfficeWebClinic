# 61 Component Diagnostics Dashboard

## Fitur
Dashboard Diagnostik merupakan layar internal (Development-only) yang ditujukan bagi QA dan Developer untuk memantau kesehatan seluruh blok *UI Composition* secara *real-time*.

## Rute Akses
- `/diagnostics/components`: Menampilkan tabel seluruh ViewComponent yang berhasil di-*load* oleh `ApplicationPartManager` ASP.NET Core, beserta asal Namespace dan Assembly.
- `/diagnostics/validation`: Menjalankan fungsi `RegistryValidatorService` dan memaparkan *Registry Coverage Report* (Kompilasi Metadata vs Realita).

Tampilan ini meminjam *Layout* utama aplikasi, sehingga tidak melanggar *Route Contract* sistem utama dan bisa dipanggil kapan saja saat siklus *development* berlangsung.
