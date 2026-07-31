# 54 Runtime Diagnostics Design

## Konsep
*Runtime Diagnostics* adalah sebuah panel overlay *collapsible* (dapat dilipat) yang hanya muncul pada mode *Development* di area bawah layar (seperti *footer bar* menempel di bawah). Panel ini bertindak sebagai alat bantu forensik real-time bagi para developer, memastikan bahwa lapisan komposisi UI berjalan transparan.

## Spesifikasi Data yang Ditampilkan
Pada saat rendering berjalan, *Diagnostics Panel* akan membaca dan menampilkan status *context* saat ini:

1. **Current Route**: Diambil dari `Context.Request.Path` (Contoh: `/Patient/Create`).
2. **Current Controller**: Diambil dari `RouteData.Values["controller"]`.
3. **Current Action**: Diambil dari `RouteData.Values["action"]`.
4. **Current View**: View Template yang diinstruksikan oleh Controller (Contoh: `Templates/MasterDetail`).
5. **Current Layout**: Layout utama yang membungkus (Contoh: `_RegionLayout`).
6. **Current Metadata**: Menampilkan JSON ringkas dari `Model` (`UIMetadata`).
7. **Current Composition**: Menampilkan jumlah komponen pada tiap region (North, West, Center, East, South).
8. **Current Region**: Konteks loop region yang sedang digambar (Contoh: *Iterating Center Region*).
9. **Current Template**: Template pendukung (bila ada) di dalam region.
10. **Current Component**: Nama ID komponen yang sedang atau baru saja dipanggil via `InvokeAsync`.

## Rencana Implementasi (Hanya Desain)
Panel dapat dibangun sebagai sebuah `ViewComponent` khusus (misal: `DiagnosticOverlayViewComponent`) yang disuntikkan secara statis di paling akhir file `_Layout.cshtml`. Panel ini akan menginspeksi tipe dari `Model` secara dinamis. Jika tipe Model adalah `UIMetadata`, maka ia akan membongkar seluruh informasi state internal (Composition, Data, ComponentId) dan merendernya dalam UI berbentuk tabel atau *JSON tree* yang rapi.
