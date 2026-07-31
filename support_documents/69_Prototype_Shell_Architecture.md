# 69 Prototype Shell Architecture

## Konsep Reusable Shell
Membuat puluhan *ViewComponent* secara paralel untuk seluruh menu yang belum melalui *UI Parity Verification* dengan versi Desktop adalah sebuah pemborosan (*waste*). 

Oleh karena itu, arsitektur *Prototype Shell* menggunakan sejumlah kecil komponen cangkang (*shell*) yang bisa dipakai ulang (reusable) di berbagai modul:
1. `PrototypeDashboard`
2. `PrototypeList`
3. `PrototypeMasterDetail`
4. `PrototypeTransaction`
5. `PrototypeReport`
6. `PrototypeAdministration`

## Integrasi Controller
Setiap *Controller* baru (seperti `AppointmentController`, `BillingController`, dll.) tidak perlu mengandung logika apa pun. Mereka murni bertindak sebagai penyedia *Metadata* (Model) yang menunjuk ke *Template* dan *ComponentId* dari salah satu *Shell* di atas. 

Pendekatan ini menjamin:
- Seluruh rute dapat diklik (tidak ada HTTP 404).
- Tidak ada crash (tidak ada HTTP 500) karena komponen dijamin eksis.
- Tampilan cukup memadai bagi user untuk memahami tata letak dasar, menu, breadcrumb, dan navigasi utama sebelum *Business Logic* ditambahkan.
