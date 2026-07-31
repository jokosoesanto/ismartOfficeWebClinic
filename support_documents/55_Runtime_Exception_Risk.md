# 55 Runtime Exception Risk Assessment

## Analisis Risiko Arsitektur Metadata Driven
Membangun UI secara dinamis (Metadata Driven UI) memberikan fleksibilitas luar biasa di mana satu Layout tunggal (`_RegionLayout`) dapat me-render ratusan variasi layar. Namun, arsitektur ini memindahkan beban verifikasi dari *Compile-Time* menjadi *Runtime*. 

Bila menggunakan pendekatan konvensional Razor View yang di-hardcode, memanggil komponen fiktif seperti `<vc:patient-form />` akan menyebabkan peringatan saat kompilasi atau gagal Build. Pada arsitektur Composition, pemanggilan dilakukan menggunakan *string* `InvokeAsync("PatientForm")` di dalam loop dinamis. Kesalahan ketik (typo) maupun keterlambatan pendaftaran komponen (*late registration* akibat Hot Reload) berisiko fatal langsung merusak seluruh page menjadi HTTP 500, bukan sekadar melompati area tersebut.

## Rencana Mitigasi Jangka Pendek
Oleh karena itu, komponen `ComponentRegistry` merupakan *Single Point of Failure (SPOF)* pada framework kita.
- **Wajib Lulus Hot Reload**: Semua penyesuaian guard, seperti blok `try/catch` pada validator komposisi, wajib dijamin terkompilasi dan beroperasi di *memory thread* tanpa hambatan *file lock*.
- **Diagnostic UI (Empty State)**: Arsitektur tidak boleh percaya sepenuhnya pada `InvokeAsync`. Apabila gagal, sistem harus mampu menjebak (*trap*) exception tersebut, menetralisirnya, dan merender "mercu suar" error spesifik di titik tersebut tanpa mempengaruhi region North, West, East, maupun South. Hal ini mencegah pengguna akhir maupun QA melihat layar putih polos atau pesan server error 500.
