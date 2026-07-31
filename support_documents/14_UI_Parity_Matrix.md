# 14 UI Parity Matrix

Dokumen ini memetakan komponen antarmuka yang ada pada aplikasi Desktop dengan komponen ekuivalen pada Web Design System yang baru, serta menganalisis kesenjangan (_gap_) yang perlu diselesaikan.

| Desktop Component (WinForms/3rd Party) | Web Design System Equivalent (Bootstrap 5 / Custom) | Reusable Component Required | Gap / Improvement Needed |
| -------------------------------------- | --------------------------------------------------- | --------------------------- | ------------------------ |
| **DataGrid / DataGridView**            | HTML Table dengan styling Bootstrap                 | `<clinic-datatable>`        | Butuh fitur server-side pagination & sort (Datatables.net / grid.js). |
| **Form Popup / ShowDialog**            | Bootstrap Modal (`.modal`)                          | `<clinic-modal>`            | Manajemen z-index dan backdrop untuk modal bersarang (_nested modals_). |
| **TabControl**                         | Bootstrap Nav Tabs (`.nav-tabs`)                    | `<clinic-tabs>`             | Harus mendukung deep-linking (URL hash) agar state tab tidak hilang saat refresh. |
| **GroupBox**                           | Card Bootstrap (`.card`)                            | `<clinic-card>`             | Desain Card sudah mengakomodir border dan judul, menggantikan GroupBox yang kaku. |
| **Toolbar (ToolStrip)**                | Bootstrap Button Group / Action Bar                 | `<clinic-actionbar>`        | Dibuat *sticky* di atas/bawah halaman. |
| **ComponentGo.Calendars**              | FullCalendar.js / Toast UI Calendar                 | `<clinic-scheduler>`        | **High Gap**: Harus mengintegrasikan library JS pihak ketiga untuk menggantikan ComponentGo. |
| **ActiveReports Viewer**               | HTML PDF Embed (`<object>`) / Report Server UI      | `<clinic-report-viewer>`    | Laporan harus di-generate via backend menjadi PDF sebelum di-_stream_ ke UI. |
| **TeethSurface / JoeCeph32 UI**        | HTML5 Canvas / WebGL Custom Editor                  | `<clinic-odontogram>`       | **Very High Gap**: Komponen native tidak dapat di-porting. Harus ditulis ulang total dalam HTML5/JS. |
| **MessageBox**                         | SweetAlert2 / Bootstrap Toasts                      | `<clinic-toast>`            | UX web lebih diarahkan pada notifikasi _non-blocking_ (Toast) alih-alih popup alert biasa. |
