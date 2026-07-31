# 62 Runtime Fallback

## Tujuan
Runtime Fallback adalah jaring pengaman (safety net) terakhir yang diletakkan pada metode `RenderComponentAsync` di `ComponentRegistry`.

## Alur Resolusi & Render
1. `Registry` meminta `ComponentResolver` untuk memeriksa ID komponen.
2. Jika status bukan `Registered`, maka proses dialihkan (*fallback*) ke metode `RenderDiagnosticFallback`.
3. Jika status `Registered` namun saat metode `InvokeAsync` dijalankan ternyata melempar *Exception* (misal karena *View* korup atau internal error pada komponen), exception ditangkap (`catch`) dan dialihkan kembali ke *fallback*.

## UI Fallback
UI Fallback menggunakan `TagBuilder` untuk membentuk peringatan (alert) *inline* yang mencolok (menggunakan kelas `alert-danger` dari Bootstrap). Informasi yang disajikan:
- Pesan Error/Status
- Expected Namespace, Assembly, View
- Stack trace/Inner message jika tersedia.

Ini memastikan aplikasi tidak hancur seketika (HTTP 500) melainkan hanya area yang bersangkutan saja yang digantikan oleh kotak peringatan merah, sedangkan *Header, Sidebar, Footer* dan region yang sukses tetap bisa diakses.
