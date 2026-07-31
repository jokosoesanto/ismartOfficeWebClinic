# 77 Template Root Cause

Berdasarkan *Tracing* dan *Discovery Analysis*, berikut adalah pembuktian objektif dari seluruh pertanyaan forensik:

**1. Mengapa MVC mencari `Views/Home/...` dan `Views/Shared/...`?**
Ini bukan bug. Ini adalah perilaku *by-design* dari *ASP.NET Core Razor View Engine*. Saat dipanggil menggunakan `return View("Templates/Dashboard")` dari dalam `HomeController`, *engine* akan secara otomatis mengkombinasikan rute relatif ini dengan folder konvensinya.

**2. Apakah file memang tidak ada?**
**BENAR**. File `Dashboard.cshtml` di dalam folder `Templates` benar-benar tidak ada di *file system*.

**3. Apakah metadata salah?**
**TIDAK**. Metadata tidak salah dan sudah dirancang sesuai rencana (*Prototype Architecture*). Kesalahannya adalah *Contract Mismatch*; dokumentasi rancangan (desain awal) menyebutkan ada template "Dashboard", sehingga kontroler memintanya, padahal developer *foundation* lupa membuatkan file-nya pada tahapan sebelumnya.

**4. Apakah resolver salah?**
**TIDAK**. *Component Resolver* dan *Component Registry* (yang di-hardening sprint lalu) bertugas memeriksa `ViewComponent`, bukan `Razor View` (Template). Dalam kasus ini, *request* jatuh di lapisan yang lebih luar (sebelum komponen sempat dipanggil).

**5. Apakah convention / konfigurasi View Engine berubah?**
**TIDAK**. *appsettings.json* dan *Program.cs* tidak mengalami perubahan yang memecahkan konvensi standar MVC.

## Kesimpulan Akar Masalah (Root Cause)
Ini murni kasus **"Unimplemented Template View"**. Sprint *Prototype Shell* membuat serangkaian Controller yang mereferensikan berbagai *Template Path* fiktif (karena hanya didasarkan pada rancangan tertulis, tanpa verifikasi fisik file). Karena MVC menuntut file Razor View `.cshtml` itu harus ada di *disk*, request langsung menghasilkan HTTP 500.
