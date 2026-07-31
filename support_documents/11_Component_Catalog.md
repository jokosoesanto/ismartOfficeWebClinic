# 11 Component Catalog

## Overview
Penggunaan komponen standar bertujuan untuk mencegah _copy-paste_ sintaks HTML yang redundan ke berbagai _Views_, yang akan sangat menyulitkan di proses perancangan _business logic_ di kemudian hari. Komponen diimplementasikan menggunakan arsitektur **TagHelpers** dari ASP.NET Core MVC.

## Daftar Komponen Aktif

### 1. `clinic-card` (Tag Helper)
- **Tujuan**: Membuat kerangka _Card_ standar untuk menampilkan grup informasi (misalnya: Informasi Pasien, Statistik Hari Ini, Graf).
- **Atribut**: 
  - `title`: String. Menjadi Header card.
  - `icon`: String (kelas Bootstrap Icon, misal `bi-people`). Tampil di sebelah teks title.
- **Penggunaan**:
  ```html
  <clinic-card title="Total Patients" icon="bi-people">
      <p>Content Goes Here</p>
  </clinic-card>
  ```
- **Kelebihan**: Mengabstraksi struktur Bootstrap Card standar `<div class="card"><div class="card-header">...</div><div class="card-body">...</div></div>`. Jika _design system_ ke depan mengharuskan perubahan margin pada kartu, Anda cukup mengubah `CardTagHelper.cs` (mengubahnya di 1 lokasi, bukan 200 views).

## Rencana Komponen Mendatang (Sprint Berikutnya)
- `clinic-modal`
- `clinic-button` (dengan loader spinner internal)
- `clinic-form-group`
