# 58 Component Diagnostics Service

## Peran
`IComponentDiagnosticsService` bertindak sebagai fasad terpusat untuk segala urusan diagnostik UI Framework. Service ini tidak terikat pada *Startup* saja, namun bisa dipanggil secara manual (on-demand) dari Controller maupun service internal lain.

## Kemampuan
1. **GetAllComponents()**: Membaca manifest lengkap dari seluruh komponen yang terdaftar secara internal pada *ApplicationPartManager* di ASP.NET.
2. **ValidateMetadata(UIMetadata)**: Melakukan ekstraksi hierarki (North, West, Center, East, South) dan meresolusikan seluruh komponen yang diminta di dalamnya terhadap ketersediaan aktual. Ini memungkinkan kita mendeteksi *Missing Component* sedini mungkin di level metadata.
