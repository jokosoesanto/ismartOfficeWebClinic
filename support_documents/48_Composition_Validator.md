# 48 Composition Validator

## Tujuan
Memvalidasi eksistensi dan validitas eksekusi _ViewComponent_ yang didikte oleh UIMetadata, sehingga kesalahan pengetikan nama komponen tidak menyebabkan sistem macet (*crash*) secara brutal (HTTP 500 menyeluruh).

## Implementasi
Validator ini dipasang langsung pada `ComponentRegistry.cs` menggunakan *Exception Handling Guard*:
```csharp
try 
{
    return await componentHelper.InvokeAsync(component.ComponentId, parameters);
}
catch (Exception ex)
{
    // Mengembalikan Diagnostic View Component
}
```
Apabila `InvokeAsync` gagal menemukan komponen atau gagal me-render karena parameter _mismatch_, sistem akan melokalisir *error* tersebut ke dalam sebuah kotak merah (Alert Danger) khusus pada area komponen bersangkutan, tanpa merusak area layar lainnya.
