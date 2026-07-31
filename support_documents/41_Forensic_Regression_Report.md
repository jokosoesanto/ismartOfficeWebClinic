# 41 Forensic Regression Report

## Executive Summary
Dokumen ini merupakan laporan investigasi forensik terhadap dua *regression bug* yang ditemukan pada saat pelaksanaan Manual UAT untuk implementasi *Metadata Driven UI Composition Framework* pada modul **Patient**.

## Scope of Investigation
Investigasi difokuskan pada:
1. **Regression 1**: Terjadinya `InvalidOperationException: RenderBody has not been called` saat membuka menu **All Patients** (`/Patient`).
2. **Regression 2**: Munculnya **halaman kosong (blank page)** saat membuka menu **Add Patient** (`/Patient/Create`).

## Metodologi
- **Source Code Analysis**: Memeriksa file Razor Views (`_RegionLayout.cshtml`, `MasterList.cshtml`, `MasterDetail.cshtml`) dan Controller (`PatientController.cs`).
- **Layout Hierarchy Analysis**: Melacak alur eksekusi dari Controller -> View Template -> Layout -> ViewComponent.
- **Dependency Analysis**: Memeriksa siklus hidup rendering layout di ASP.NET Core.

## Kesimpulan Awal
- **Regression 1** murni disebabkan oleh pelanggaran *lifecycle* dari Razor Layout Engine di ASP.NET Core yang mewajibkan pemanggilan `RenderBody()`.
- **Regression 2** murni disebabkan oleh *data starvation* (kurangnya injeksi komponen pada metadata) yang dipadukan dengan sifat View Template kita yang merupakan cangkang kosong (hanya me-render HTML comment).

Rincian *Root Cause Analysis* (RCA) untuk masing-masing temuan dijabarkan pada dokumen terpisah (`42_RenderBody_RootCause.md` dan `43_AddPatient_RootCause.md`).
