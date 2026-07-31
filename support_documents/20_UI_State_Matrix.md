# 20 UI State Matrix

Aplikasi web memiliki berbagai macam _state_ (status) komponen yang bergantung pada aksi pengguna atau kondisi respons dari server. Berikut matriks penanganan status antarmuka:

| Element / Component | Default State | Action / Trigger | Loading State | Success State | Error State |
| ------------------- | ------------- | ---------------- | ------------- | ------------- | ----------- |
| **Grid / Table**    | Data terisi atau Teks "No Data" | Sorting, Paging, Searching | Teks/Spinner "Memuat..." di tengah grid (Tabel lama memudar) | Tabel diperbarui dengan baris data baru. | Teks merah "Gagal memuat data", ikon Retry. |
| **Primary Button**  | Enabled, warna sesuai _Theme_ | Diklik untuk Submit data / form | Disabled, teks diganti "Processing..." beserta ikon Spinner. | Tombol kembali Enabled, muncul Toast Success. | Tombol kembali Enabled, muncul validasi error di bawah input. |
| **Form Input**      | Kosong, garis abu-abu (border-color default) | Ketik (OnBlur / Submit) | N/A | Kotak border warna hijau (valid). | Kotak border warna merah (invalid), pesan error warna merah. |
| **Sidebar Menu**    | Terbuka (_Expanded_) | Klik Icon Hamburger | N/A | Menyusut (_Collapsed_), ikon membesar. | N/A |
| **Theme Switcher**  | Menunjukkan nama tema aktif | Pilih tema baru | N/A | Tema CSS otomatis diterapkan. | N/A |
| **Modal Dialog**    | Tersembunyi | Klik aksi (Delete/View) | Tampilan Skeleton dalam body Modal. | Data tampil / modal tertutup jika save. | Pesan alert di dalam modal. |

Pengembang (_Frontend Developer_) wajib memastikan seluruh transisi ini berjalan halus untuk mengurangi kecemasan pengguna (*user anxiety*) akibat delay latensi jaringan yang tidak dialami pada aplikasi desktop lokal.
