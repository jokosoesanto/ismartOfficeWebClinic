# 49 Rendering Lifecycle

Dokumen ini mendeskripsikan aturan baku siklus rendering (Rendering Lifecycle) pada arsitektur *Metadata Driven UI Framework*.

1. **Routing & Controller**: Request tiba di ASP.NET Core Router dan dikirim ke Controller.
2. **Metadata Formulation**: Controller tidak melakukan operasi data, melainkan menyusun struktur `UIMetadata` (mencakup judul, modul, dan `UIComposition`).
3. **Template Assignment**: Controller mengembalikan View Template (misalnya: `MasterList.cshtml`).
4. **Layout Resolution**: File Template me-_resolve_ Master Layout ke `_RegionLayout.cshtml`.
5. **Layout Execution**: `_RegionLayout` mengevaluasi koleksi region di dalam `UIComposition`.
6. **Guard Check**: Jika koleksi region sepenuhnya kosong, maka *Runtime Guard* akan aktif. Pemanggilan `@RenderBody()` diabaikan secara paksa menggunakan `IgnoreBody()`.
7. **Component Injection**: Jika terdapat komponen, maka Master Layout akan menginstruksikan `ComponentRegistry` untuk melokalisasi dan me-render ViewComponent satu demi satu ke dalam HTML string.
8. **RenderBody Resolution**: Sesuai dengan aturan ASP.NET Razor, `@IgnoreBody()` wajib dipanggil manakala jalur eksekusi tidak memanggil `@RenderBody()` untuk memuaskan tuntutan engine.

Dengan adanya *Rendering Lifecycle* yang diawasi ini, sistem kebal terhadap anomali Layout.
