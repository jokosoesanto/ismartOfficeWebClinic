# 18 Wireframe Catalog (UI Structure Specification)

Dokumen ini mendefinisikan layout standar untuk setiap _pattern_ halaman pada aplikasi web, yang diubahsuaikan dari form Desktop.

## Pattern A: Master Data List (e.g., Patient List, User List)
```text
+---------------------------------------------------------+
| [Header] [Breadcrumb] [Global Search]         [Profile] |
+---------------------------------------------------------+
| [Page Title]                                            |
|                                                         |
| +-----------------------------------------------------+ |
| | [Filter Area: Dropdowns, Date Range] [Search input] | |
| +-----------------------------------------------------+ |
|                                                         |
| [Action Bar: (Add New) (Export) (Print)]                |
|                                                         |
| +-----------------------------------------------------+ |
| | DataGrid / Table                                    | |
| | [ ] ID  | Name | Status | Actions                   | |
| | [ ] 01  | John | Active | (Edit) (Del) (View)       | |
| | ...                                                 | |
| +-----------------------------------------------------+ |
| [Pagination: Prev 1 2 3 Next]       [Total: 120 items]|
+---------------------------------------------------------+
```

## Pattern B: Master Data Detail / Form (e.g., Add Patient)
```text
+---------------------------------------------------------+
| [Header] [Breadcrumb]                         [Profile] |
+---------------------------------------------------------+
| [< Back] [Page Title]                                   |
|                                                         |
| +-----------------------------------------------------+ |
| | Tabs: [Demographics] [Guardian] [Insurance]         | |
| +-----------------------------------------------------+ |
| |                                                     | |
| | First Name [________________]  DOB [__/__/____]     | |
| | Last Name  [________________]  Gender [v]           | |
| | Address    [________________]                       | |
| |                                                     | |
| +-----------------------------------------------------+ |
|                                                         |
| [Footer Action: (Cancel)                (Save Changes)] |
+---------------------------------------------------------+
```

## Pattern C: Complex Workspace (e.g., Dental Chart)
```text
+---------------------------------------------------------+
| [Header] [Breadcrumb]                         [Profile] |
+---------------------------------------------------------+
| [< Back] Patient: John Doe | DOB: 01/01/1990 (36 yo)    |
+---------------------------------------------------------+
| [Toolbar: (Caries) (Missing) (Crown) (Extract)]         |
+-----------------------+---------------------------------+
|                       |                                 |
|      CANVAS           |  Summary Panel / Added Items    |
|   (Odontogram 2D)     |  - Tooth 14: Caries ($50)       |
|                       |  - Tooth 46: Extracted ($20)    |
|                       |                                 |
|                       |  [Action: Bill to Patient]      |
+-----------------------+---------------------------------+
```

Semua pembuatan _Views_ di ASP.NET Core kelak wajib mematuhi panduan kerangka (wireframe) struktural di atas.
