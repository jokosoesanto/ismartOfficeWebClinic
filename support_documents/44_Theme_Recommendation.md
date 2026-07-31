# 44 Theme Engine Recommendation

Berdasarkan tinjauan arsitektur antarmuka dan referensi palet *Color Hunt*, direkomendasikan penyusunan ulang Theme Engine berbasis **Design Tokens (CSS Variables)**. Pendekatan ini memungkinkan *theming* yang adaptif tanpa harus mengubah struktur _class_ HTML sedikitpun.

Berikut adalah 3 usulan kandiat tema modern untuk aplikasi klinik, beserta penjabaran tokens-nya.

## 1. Candidate A: "Serene Health" (Medical Blue/Teal)
*Alasan Pemilihan*: Memberikan nuansa kepercayaan (trust), kebersihan klinis, dan ketenangan yang biasa diasosiasikan dengan fasilitas medis modern dan profesional. Sangat ramah mata untuk penggunaan berdurasi lama (layar admin/operasional).

- **Primary**: `#008080` (Teal) - Digunakan untuk tombol utama, highlight tab aktif.
- **Secondary**: `#20B2AA` (Light Sea Green) - Varian sekunder untuk badge atau aksen pendukung.
- **Accent**: `#FFA07A` (Light Salmon) - Untuk aksi non-standar, notifikasi halus.
- **Surface**: `#FFFFFF` - Kartu konten, area input.
- **Background**: `#F0F8FF` (Alice Blue) - Background aplikasi (canvas).
- **Success**: `#2E8B57` (Sea Green) - Sukses simpan/update.
- **Warning**: `#FFD700` (Gold) - Peringatan menengah.
- **Danger**: `#DC143C` (Crimson) - Error, Delete, atau Alert alergi.
- **Border**: `#E0E6ED` - Garis pembatas yang lembut.
- **Text Primary**: `#2C3E50` - Teks gelap elegan (bukan hitam murni) untuk kontras prima.
- **Text Secondary**: `#7F8C8D` - Teks petunjuk, placeholder.

## 2. Candidate B: "Clinical Dark Mode" (Night Shift)
*Alasan Pemilihan*: Sangat diminta oleh pengguna sistem rumah sakit yang sering dinas malam (Night Shift). Mengurangi *eye strain* saat cahaya ruangan redup.

- **Primary**: `#4FD1C5` (Soft Teal) - Tetap membawa nuansa medis namun lebih terang untuk kontras latar gelap.
- **Secondary**: `#38B2AC`
- **Accent**: `#F6AD55` (Soft Orange)
- **Surface**: `#2D3748` (Slate Gray) - Sebagai latar kartu (*Card*).
- **Background**: `#1A202C` (Very Dark Gray) - Dasar aplikasi.
- **Success**: `#68D391`
- **Warning**: `#F6E05E`
- **Danger**: `#FC8181`
- **Border**: `#4A5568`
- **Text Primary**: `#E2E8F0` - Teks putih pudar.
- **Text Secondary**: `#A0AEC0` - Teks deskripsi pudar.

## 3. Candidate C: "Minimalist Indigo" (Corporate Medical)
*Alasan Pemilihan*: Mengambil inspirasi dari _software enterprise_ global (misal: Stripe, Jira). Terlihat premium, serius, dan fokus pada efisiensi membaca data Grid (sangat cocok untuk Billing & ERP modul).

- **Primary**: `#5A67D8` (Indigo)
- **Secondary**: `#7F9CF5`
- **Accent**: `#F687B3` (Soft Pink) - Memberi sentuhan modern.
- **Surface**: `#FFFFFF`
- **Background**: `#F7FAFC` - Abu-abu sangat tipis.
- **Success**: `#48BB78`
- **Warning**: `#ECC94B`
- **Danger**: `#E53E3E`
- **Border**: `#EDF2F7`
- **Text Primary**: `#1A202C`
- **Text Secondary**: `#718096`

## Kesimpulan Rekomendasi
Disarankan implementasi Design Token menggunakan CSS _custom properties_ (`:root`) untuk memuat *Candidate A* sebagai tema standar (*Default*), dan *Candidate B* sebagai fungsi *Toggle Night Mode*. Seluruh *ViewComponents* dan *Layout* harus menggunakan token CSS, misal: `background-color: var(--surface); color: var(--text-primary);`.
