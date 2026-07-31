# 12 Navigation Architecture

## Masalah Hardcoding
Pada struktur _website_ tradisional, struktur _Sidebar_ dan hierarki _Breadcrumb_ biasanya di-_hardcode_ secara statis di level HTML (Razor Views). Ini menyulitkan _RBAC (Role-Based Access Control)_ yang mana menu harus dimunculkan / disembunyikan berdasarkan izin akses _User_.

## Arsitektur Provider Berbasis Data
Clinic Web menggunakan **Navigation Architecture** yang di-_feed_ melalui `INavigationProvider`.

1. **Domain Model**: Terdapat model `NavigationItem` (Id, Title, Icon, Route, Role Required, dan hirarki Children).
2. **Mock Provider**: Untuk prototype, kami membuat _Mock_ bernama `NavigationProvider.cs` di dalam _Application Layer_ yang berisi List statis memuat struktur modul. Struktur modul ini merupakan implementasi dari referensi yang ditarik dari **Discovery Report**.
3. **ViewComponents (Consumer)**:
   - `SidebarViewComponent`: Merender `NavigationItem` secara dinamis, mengaktifkan status *dropdown/collapse* berdasarkan _Current URL Request Route_.
   - `BreadcrumbViewComponent`: Melakukan penelusuran _tree_ dari `NavigationItem` untuk membangun urutan remah roti (misalnya: `Home > Administrasi > Roles`).

## Migrasi Masa Depan
Pada sprint di mana **Business Logic** dan Database diaplikasikan, `NavigationProvider` mock dapat ditukar (`Dependency Injection`) dengan `DatabaseNavigationProvider` yang menarik metadata navigasi dari tabel _Permissions_ di SQL Server / PostgreSQL. Hal ini menjamin bahwa seluruh antarmuka yang terikat kepadanya (Sidebar & Breadcrumbs) akan terus bekerja tanpa perlu refactoring komponen HTML.
