# 52 HTTP 500 Root Cause Analysis

## Pertanyaan Utama
*Mengapa `Patient List` PASS tetapi `Patient Create` HTTP 500 padahal keduanya menggunakan framework yang sama?*

## Pembuktian Perbedaan
1. **Status Komponen di Memori (ViewComponentFeatureProvider)**
   - **Patient List (PASS)**: Memanggil komponen `DataGrid`. Komponen ini sudah eksis di dalam source code sejak aplikasi pertama kali dijalankan (Startup). ASP.NET Core MVC melakukan *assembly scanning* satu kali saat booting untuk mendaftarkan seluruh ViewComponent yang ada.
   - **Patient Create (HTTP 500)**: Memanggil komponen `PatientForm`. File `PatientFormViewComponent.cs` baru saja diciptakan secara dinamis di tengah-tengah sesi berjalannya aplikasi (`dotnet watch run`). Meskipun file fisik ada dan sintaks C#-nya valid 100%, sistem registri internal ASP.NET Core tidak me-*rescan* ulang assembly untuk ViewComponent baru, sehingga runtime menganggap kelas tersebut "tidak ada" (Not Found).

2. **Kegagalan Runtime Guard**
   - Pada sprint sebelumnya, sebuah blok `try/catch` telah dirancang di `ComponentRegistry.cs` untuk mencegat error `InvalidOperationException` dan merender pesan diagnostik.
   - Namun, karena proses kompilasi ulang (Hot Reload / Build) terhalang oleh *file lock* pada executable yang sedang berjalan, perubahan kode `try/catch` tersebut belum masuk ke dalam *memory thread* yang aktif melayani request.
   - Akibatnya, exception bocor (unhandled) hingga menabrak *DeveloperExceptionPageMiddleware*, yang merender halaman HTTP 500.

## Kesimpulan
Perbedaan utama terletak pada **waktu siklus hidup komponen (Lifecycle Timing)**. `DataGrid` adalah komponen statis yang diregistrasi saat *cold boot*, sedangkan `PatientForm` adalah komponen *hot-injected* yang luput dari radar ASP.NET Core ViewComponent Provider pada runtime berjalan.
