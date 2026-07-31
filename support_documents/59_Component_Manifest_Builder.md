# 59 Component Manifest Builder

## Spesifikasi Manifest
Untuk mewujudkan visibilitas yang komprehensif, arsitektur kita memetakan objek internal `ViewComponentDescriptor` milik ASP.NET menjadi bentuk `ComponentManifest`.

### Struktur `ComponentManifest`
- **ComponentId**: Nama unik (ShortName).
- **Namespace**: Lokasi class secara struktural (misal: `Clinic.Web.ViewComponents`).
- **Assembly**: DLL penyedia komponen.
- **ExpectedViewPath**: Jalur fisik konvensional `Views/Shared/Components/{Name}/Default.cshtml`.
- **Status**: Enumerasi hasil resolusi (Registered, Missing, InvalidNamespace, dsb).
- **RegistrationSource**: Menunjukkan darimana pendaftaran berasal (default: `ApplicationPartManager`).

Manifest ini dibangun secara dinamis (*on-the-fly*) dari *registry collection*, memastikan bahwa ia tidak pernah usang dan tidak memerlukan modifikasi file statis saat ada penambahan komponen.
