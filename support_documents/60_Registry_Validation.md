# 60 Registry Validation

## Konsep
Validasi registry adalah proses auditing menyeluruh untuk mendeteksi anomali pada metadata UI. Mengingat arsitektur kita bersifat metadata-driven, kesalahan pengetikan nama komponen di metadata tidak akan terdeteksi saat *Compile-Time*.

## Mekanisme Pengecekan
`RegistryValidatorService` melakukan *Reflection* terhadap semua Controller di dalam aplikasi.
1. Ia mencari semua rute *parameterless HttpGet* yang merender `ViewResult`.
2. Jika *Model* dari *ViewResult* bertipe `UIMetadata`, metadata tersebut diekstrak.
3. Metadata dikirim ke `ValidateMetadata()` untuk menguraikan semua `ComponentId` di North, West, Center, East, South.
4. Setiap komponen divalidasi ketersediaannya terhadap daftar yang dimiliki oleh *ViewComponentDescriptorCollectionProvider*.

Hasilnya berupa `RegistryCoverageReport` yang menunjukkan:
- Mana saja metadata yang sempurna (Valid Metadata).
- Metadata mana yang merujuk pada ViewComponent fiktif (Missing Component References).
- Komponen mana yang ada di *assembly* tetapi tidak pernah digunakan di *metadata* manapun (Unused Components).
- Apakah ada komponen dengan nama duplikat (Duplicate Registrations).
