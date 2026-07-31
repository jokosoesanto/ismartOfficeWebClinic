# 16 Component Usage Map

Tabel ini menunjukkan layar mana saja yang akan menggunakan komponen UI spesifik dari _Design System_. Hal ini memastikan pemanfaatan _Reusable Components_ secara maksimal.

| Layar / Modul | `clinic-card` | `clinic-modal` | `clinic-tabs` | `clinic-datatable` | `clinic-scheduler` | Khusus (Domain Specific) |
| ------------- | :-----------: | :------------: | :-----------: | :----------------: | :----------------: | ------------------------ |
| **Dashboard** | ✅ | | | | | Chart.js (Grafik) |
| **Patient List** | ✅ | ✅ (Filter/Export) | | ✅ | | |
| **Patient Detail**| ✅ | ✅ (Alert/Confirm) | ✅ | ✅ (Tx History) | | |
| **Patient Form** | ✅ | | ✅ | | | |
| **Appt Console**| ✅ | ✅ (Call Patient)| | ✅ | | Auto-Refresh Timer |
| **Calendar** | ✅ | ✅ (Quick Add) | | | ✅ (FullCalendar)| Drag & Drop Manager |
| **Kiosk CheckIn**| ✅ | | | | | Numpad Touch UI |
| **Dental Chart**| ✅ | ✅ (Add Finding) | ✅ | | | `<clinic-odontogram>` |
| **Payment Form**| ✅ | ✅ (Payment Method)| | ✅ (Item Grid)| | Print Preview Frame |
| **Inventory** | ✅ | ✅ (Restock form) | | ✅ | | |
| **Admin Setup** | ✅ | ✅ (Edit Item) | | ✅ | | |
