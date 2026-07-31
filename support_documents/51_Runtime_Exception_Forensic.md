# 51 Runtime Exception Forensic

## Objektif
Menginvestigasi akar masalah HTTP 500 Internal Server Error pada rute `GET /Patient/Create` yang terjadi setelah implementasi Regression Fix Sprint, tanpa mengubah source code.

## Tracing Lengkap Request `GET /Patient/Create`

| Tahapan | Status | Bukti / Keterangan |
|---------|--------|--------------------|
| **Route Resolution** | **PASS** | Request berhasil di-routing ke `PatientController`, Action `Create`. |
| **Controller Action** | **PASS** | Action `Create` berhasil dieksekusi dan membentuk instansiasi `UIMetadata`. |
| **Metadata Factory** | **PASS** | `UIMetadata` terbentuk dengan `ModuleName = "Patient"` dan `Title = "Create Patient"`. |
| **Composition Engine** | **PASS** | `UIComposition` berhasil mendefinisikan `PatientForm` di area `Center`. |
| **Razor View** | **PASS** | View `Templates/MasterDetail` berhasil ditemukan dan di-render. |
| **Layout Resolution** | **PASS** | View berhasil me-resolve layout ke `_RegionLayout.cshtml`. |
| **Dependency Injection**| **PASS** | `IComponentRegistry` berhasil di-inject ke dalam layout. |
| **Component Registry** | **FAIL** | Terjadi kegagalan saat mencoba memanggil `InvokeAsync` pada `DefaultViewComponentHelper`. |
| **ViewComponent** | **FAIL** | Engine tidak dapat menemukan komponen bernama `PatientForm`. |
| **Runtime Rendering** | **FAIL** | Exception merambat naik hingga menghentikan pipeline MVC, menghasilkan HTTP 500. |

## Kesimpulan Forensik
Exception tidak terjadi pada kode bisnis atau logika rendering Razor, melainkan pada tahap **Component Discovery** internal ASP.NET Core saat runtime mencoba menemukan kelas ViewComponent bernama `PatientForm`.
