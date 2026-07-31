# 10 Theme Architecture

## Konsep
Aplikasi mengusung _Theme Engine_ tanpa pergantian file CSS eksternal (.css swap). Alih-alih melakukan _swap_, aplikasi menempelkan atribut `data-theme` pada tag `<html>`.

Setiap deklarasi tema berada di dalam file `themes.css` dalam blok pemilih atribut seperti `[data-theme="nama-tema"]`. Variabel yang berada di dalamnya akan secara otomatis menimpa (_override_) nilai default yang berada pada `:root`.

## Daftar Tema (6 Theme Ready)
1. **Medical Blue (Default)** - Standar rumah sakit / klinik, mengedepankan warna biru medikal yang steril.
2. **Medical Green** - Tema alternatif dengan aksen hijau, memberi kesan asri dan _wellness_.
3. **Corporate Indigo** - Desain korporat premium yang modern (menggunakan dominasi _indigo_).
4. **Navy Professional** - Warna gelap dan biru *navy* khas aplikasi B2B dan enterprise dashboard.
5. **Dark Mode** - Mode gelap untuk lingkungan minim cahaya dan ergonomi mata.
6. **Minimal Light** - Kontras tinggi (hitam/putih) tanpa banyak gradasi, untuk fokus kecepatan dan keterbacaan data numerik.

## Script Engine
Tema diganti secara dinamis via JavaScript murni. Pilihan pengguna disimpan ke dalam `localStorage` sehingga preferensi tema tetap tersimpan (_persisted_) walau browser di-refresh. 

```javascript
document.documentElement.setAttribute('data-theme', theme);
localStorage.setItem('clinic-theme', theme);
```
