# 75 Template Contract Verification

## Hasil Verifikasi Eksplisit

- **Template Name dari Metadata/Controller**: Kontroler (e.g. `HomeController`) mengembalikan nama literal `"Templates/Dashboard"` via metode `View(string viewName, object model)`.
- **Nama View yang Diminta MVC**: `"Templates/Dashboard"`.
- **Lokasi Fisik View yang Sebenarnya Ada**: Di dalam folder arsitektur kita (`Views/Shared/Templates/`), hanya terdapat dua file:
  1. `MasterList.cshtml`
  2. `MasterDetail.cshtml`
- **Konvensi Folder Template Framework**: ASP.NET Core MVC meresolusi view `Templates/Dashboard` dengan mencari di dua folder bawaan:
  - `Views/{ControllerName}/{ViewName}.cshtml` (yaitu: `Views/Home/Templates/Dashboard.cshtml`)
  - `Views/Shared/{ViewName}.cshtml` (yaitu: `Views/Shared/Templates/Dashboard.cshtml`)
- **Mekanisme Controller**: Menggunakan `return View(...)`. Ini merupakan mekanisme standar Razor View, bukan pemanggilan *ViewComponent* secara langsung.
- **Tipe Template**: Template dituntut berupa *Razor View* (`.cshtml`), karena ia bertugas sebagai kanvas (`View`) pembungkus (*host*) yang memanggil `ViewComponent` internal kita (berdasarkan arsitektur metadata-driven).
