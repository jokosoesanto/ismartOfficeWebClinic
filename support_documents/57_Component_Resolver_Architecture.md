# 57 Component Resolver Architecture

## Konsep
`IComponentResolver` dirancang untuk memverifikasi apakah sebuah string ID komponen benar-benar memiliki referensi konkret di dalam koleksi `IViewComponentDescriptorCollectionProvider` milik ASP.NET Core.

## Alur Kerja
1. Menerima `ComponentId`.
2. Mencari kecocokan (*OrdinalIgnoreCase*) di dalam daftar *ViewComponents*.
3. Jika Count == 0, mengembalikan status `Missing`.
4. Jika Count > 1, mengembalikan status `Duplicate`.
5. Jika Count == 1, melakukan ekstrak Namespace dan Assembly untuk memastikan keabsahannya (menghasilkan status `Registered` atau `InvalidNamespace`).

Resolusi ini krusial karena berjalan **sebelum** metode `InvokeAsync` dipanggil, menghindarkan eksekusi pada komponen yang dijamin pasti gagal.
