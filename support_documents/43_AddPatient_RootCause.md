# 43 AddPatient Root Cause Analysis

## Symptoms
Ketika pengguna membuka rute `/Patient/Create` (Menu Add Patient), aplikasi menampilkan halaman yang seluruh isinya kosong putih (blank). Tidak ada error yang dilemparkan, namun tampilan benar-benar hilang selain *header* navigasi utama (jika _Layout_ utamanya masih ter-render).

## Reproduction Steps
1. Jalankan aplikasi web.
2. Navigasikan ke `http://localhost:<port>/Patient/Create`.
3. Layar halaman area konten akan tampak kosong.

## Stack Trace
Tidak ada stack trace atau HTTP 500. *Status Code* adalah 200 OK.

## Root Cause
Pada `Clinic.Web/Controllers/PatientController.cs`, eksekusi Action `Create()` adalah sebagai berikut:
```csharp
[HttpGet("Create")]
public IActionResult Create()
{
    return View("Templates/MasterDetail", new UIMetadata { Title = "Create Patient", ModuleName = "Patient" });
}
```
Ketika menginisialisasi `new UIMetadata`, properti `Composition` secara default diinisialisasi dengan kumpulan `List<UIComponent>` kosong:
```csharp
public UIComposition Composition { get; set; } = new(); 
// Artinya North = empty, Center = empty, dll.
```
Karena tidak ada komponen satupun yang disuntikkan ke dalam region manapun (North, South, East, West, Center kosong), `_RegionLayout` akan mencoba mengeksekusi blok `else` pada Center:
```html
else
{
    @RenderBody()
}
```
Namun, Template `MasterDetail.cshtml` adalah file kosong yang tidak memiliki HTML apapun (hanya komentar `<!-- MasterDetail Template -->`). Oleh karena itu, output HTML yang dihasilkan hanyalah komentar kosong tersebut. Tidak ada komponen UI yang di-_render_ karena komponen _View_ sepenuhnya bergantung pada konfigurasi `Composition`, sementara konfigurasinya kosong (*data starvation*).

## Affected Files
- `C:\Users\cipac\Documents\Projects\ismartOfficeWebClinic\Clinic.Web\Controllers\PatientController.cs` (Faktor Penyebab).
- `C:\Users\cipac\Documents\Projects\ismartOfficeWebClinic\Clinic.Web\Views\Shared\Templates\MasterDetail.cshtml` (Kekosongan Konten).

## Impact Analysis
Setiap layar yang tidak mengisi `Composition` dalam `UIMetadata`-nya akan berakibat menjadi halaman kosong. Walau tidak membuat sistem crash, *User Experience* menjadi sangat membingungkan karena tidak ada indikasi apapun.

## Risk Analysis
- **Severity**: High (Mencegah fitur Add Patient berjalan sepenuhnya).
- **Likelihood**: 100% pada _route_ yang belum dikonfigurasi komposisinya.

## Proposed Fix
Modifikasi Action `Create()` di `PatientController.cs` agar mengisi `Composition` dengan komponen-komponen form input yang relevan, selayaknya halaman *Detail*. Sebagai contoh:
```csharp
[HttpGet("Create")]
public IActionResult Create()
{
    var meta = new UIMetadata
    {
        Title = "Create Patient",
        ModuleName = "Patient",
        Composition = new UIComposition
        {
            Center = new List<UIComponent>
            {
                new UIComponent { ComponentId = "PatientForm" } // Atau Tab komponen
            }
        }
    };
    return View("Templates/MasterDetail", meta);
}
```

## Estimated Regression Risk
**Low**. Dengan memberikan komposisi komponen yang tepat, *Composition Engine* akan memiliki instruksi jelas mengenai _ViewComponent_ apa yang harus dirender di layar, mencegah halaman kosong. Perbaikan hanya melokalisir pada layer Controller.
