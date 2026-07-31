# 45 Regression Risk Assessment

## Pendahuluan
Dokumen ini merupakan bentuk asesmen risiko terhadap perbaikan (Fix) yang nantinya akan diimplementasikan terhadap *Regression 1* (RenderBody Error) dan *Regression 2* (AddPatient Blank Page). Analisis ini dibuat tanpa melakukan modifikasi kode sedikitpun pada sprint ini.

## Analisis Risiko Arsitektural
Mengingat aplikasi saat ini sedang berada pada fase **Framework Freeze** (lihat dokumen `36_Framework_Freeze.md`), setiap perbaikan yang menyentuh `_RegionLayout.cshtml` atau *Component Registry* memiliki risiko regresi arsitektural.

### Risiko Perbaikan Regression 1 (RenderBody Fix)
- **Komponen Terdampak**: `_RegionLayout.cshtml`
- **Risiko**: Low. Penambahan `IgnoreBody()` di dalam kontrol aliran Layout adalah instruksi valid bagi Razor engine.
- **Pengecekan Ketergantungan**: Fitur ini tidak mengubah desain UI, dan tidak mematahkan kontrak CSS. *Center Region* tetap ter-_render_ dengan benar. Jika kelak ada fitur yang menaruh komponen di Center DAN mengharapkan `RenderBody` berjalan bersamaan, maka akan terjadi konflik, tapi hal ini sudah tidak diizinkan oleh arsitektur UI Composition saat ini.

### Risiko Perbaikan Regression 2 (AddPatient Fix)
- **Komponen Terdampak**: `PatientController.cs` (Action `Create`).
- **Risiko**: Very Low.
- **Pengecekan Ketergantungan**: Ini hanyalah masalah inisialisasi properti objek (mengisi *state* pada `UIMetadata`). Tidak ada struktur *engine* utama yang berubah. Sifatnya terisolasi hanya pada _Controller Action_ tersebut. Ini bahkan hampir dikategorikan sebagai "Missing Data" ketimbang "Framework Bug".

## Mitigasi Jangka Panjang
Untuk mencegah regresi serupa di modul lain (Billing, Appointment):
1. **Layout Validator**: Terapkan peringatan di `UIMetadata` (misalnya di konstruktor) yang mewajibkan developer mem-_pass_ setidaknya satu `UIComponent` ke dalam `Composition`. Jika `Composition` sepenuhnya kosong, lemparkan error `ArgumentException` agar ketahuan lebih dini ketimbang menghasilkan blank page saat _runtime_.
2. **Standardization**: Selalu masukkan `@IgnoreBody()` di master template jika kita 100% menggunakan arsitektur komposisi komponen dan mendedikasikan halaman sepenuhnya bagi region.

Semua penyesuaian di atas akan dijalankan apabila Sprint Implementasi Fix resmi disetujui.
