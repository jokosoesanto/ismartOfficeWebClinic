$connStr = "Data Source=C:\Users\cipac\Documents\DB\WebClinic.sqlite3;"
Add-Type -Path "C:\Users\cipac\.nuget\packages\microsoft.data.sqlite.core\10.0.2\lib\net10.0\Microsoft.Data.Sqlite.dll"
Add-Type -Path "C:\Users\cipac\.nuget\packages\sqlitepclraw.core\2.1.12\lib\net8.0\SQLitePCLRaw.core.dll"

# Use System.Data.SQLite via ADO.NET if available, otherwise fall back to simple approach
try {
    [System.Reflection.Assembly]::LoadWithPartialName("System.Data.SQLite") | Out-Null
    $conn = New-Object System.Data.SQLite.SQLiteConnection("Data Source=C:\Users\cipac\Documents\DB\WebClinic.sqlite3;")
    $conn.Open()
    $cmd = $conn.CreateCommand()
    $cmd.CommandText = "SELECT COUNT(*) FROM Procedures"
    Write-Output "Procedure record count: $($cmd.ExecuteScalar())"
    $cmd.CommandText = "SELECT sql FROM sqlite_master WHERE type='table' AND name='Procedures'"
    Write-Output "Table DDL: $($cmd.ExecuteScalar())"
    $cmd.CommandText = "SELECT name FROM sqlite_master WHERE sql LIKE '%Procedures%' AND type='table' AND name != 'Procedures'"
    $r = $cmd.ExecuteReader()
    while($r.Read()) { Write-Output "FK Reference from: $($r.GetString(0))" }
    $r.Close()
    $conn.Close()
} catch {
    Write-Output "System.Data.SQLite not available: $_"
    Write-Output "Trying direct sqlite3 CLI..."
}
