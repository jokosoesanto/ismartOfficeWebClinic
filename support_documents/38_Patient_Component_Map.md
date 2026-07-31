# 38 Patient Component Map

Berikut adalah pemetaan _ViewComponent_ ke fungsionalitas UI Modul Patient. Seluruh komponen ini berada di `Clinic.Web/ViewComponents`.

| Component Name | Description | Registry Key | Active Region | Is Lazy |
| -------------- | ----------- | ------------ | ------------- | ------- |
| **DataGrid** | Tabel daftar pasien standar (_Mock_ dari ExpandoObject). | `DataGrid` | Center | False |
| **PatientSummary** | Panel profil pasien (Nama, ID, Action buttons). | `PatientSummary` | North | False |
| **PatientTabs** | Induk tabulasi untuk panel-panel Detail Pasien. | `PatientTabs` | Center | False |
| **MedicalAlert** | Panel peringatan klinis khusus (Alergi dll). | `MedicalAlert` | East | False |

Komponen di dalam Tab (Guardian, Insurance) saat ini di-*embed* langsung ke dalam template Tab Demografi, sedangkan bagian "Treatment History" dikonfigurasi sebagai _Lazy Load Placeholder_ (akan ditarik via AJAX).
