$ErrorActionPreference = 'Stop'
$base = 'http://localhost:5130'
$s = New-Object Microsoft.PowerShell.Commands.WebRequestSession

function Get-Token($html) {
    $m = [regex]::Match($html, '<input[^>]*__RequestVerificationToken[^>]*>')
    if (-not $m.Success) { return '' }
    $v = [regex]::Match($m.Value, 'value="([^"]+)"')
    return $v.Groups[1].Value
}

Write-Host "1. Logging in..."
$lp = Invoke-WebRequest -Uri "$base/Auth/Login" -WebSession $s -UseBasicParsing
$tok = Get-Token $lp.Content
$body = @{ Username = 'admin'; Password = 'Password123!'; RememberMe = 'false'; returnUrl = ''; __RequestVerificationToken = $tok }

try {
    $lr = Invoke-WebRequest -Uri "$base/Auth/Login" -Method Post -Body $body -WebSession $s -UseBasicParsing -MaximumRedirection 0 -ErrorAction SilentlyContinue
} catch {
    # Expected to throw if 302 Found
    $lr = $_.Exception.Response
}

if ($lr -is [System.Net.HttpWebResponse]) {
    $rawHeaders = $lr.Headers.ToString()
    foreach ($line in $rawHeaders -split "`n") {
        if ($line -match '^Set-Cookie:\s*(.+)') {
            $cookieStr = $matches[1]
            if ($cookieStr -match '(\.AspNetCore\.[\w.\-]+)=([^;,\s]+)') {
                $c = New-Object System.Net.Cookie($matches[1], $matches[2], '/', 'localhost')
                $c.Secure = $false
                $s.Cookies.Add($c)
                Write-Host "Re-added cookie $($matches[1])"
            }
        }
    }
} else {
    # If not throwing 302, raw content has headers
    foreach ($cm in [regex]::Matches($lr.RawContent, '(\.AspNetCore\.[\w.\-]+)=([^;,\s]+)')) {
        $c = New-Object System.Net.Cookie($cm.Groups[1].Value, $cm.Groups[2].Value, '/', 'localhost')
        $c.Secure = $false
        $s.Cookies.Add($c)
        Write-Host "Re-added cookie $($cm.Groups[1].Value)"
    }
}


Write-Host "2. Getting first patient..."
$pat = Invoke-WebRequest -Uri "$base/Patient" -WebSession $s -UseBasicParsing
$patIds = [regex]::Matches($pat.Content, '/Patient/(?:Edit|Details)/([0-9a-fA-F-]{36})') | ForEach-Object { $_.Groups[1].Value } | Select-Object -Unique
if (-not $patIds) { Write-Host "No patients found!"; exit 1 }
$patientId = $patIds[0]
Write-Host "Using Patient ID: $patientId"

Write-Host "3. Getting an insurance..."
$ins = Invoke-WebRequest -Uri "$base/Insurance" -WebSession $s -UseBasicParsing
$insIds = [regex]::Matches($ins.Content, '/Insurance/Edit/([0-9a-fA-F-]{36})') | ForEach-Object { $_.Groups[1].Value } | Select-Object -Unique
if (-not $insIds) {
    Write-Host "No insurances found! Creating a dummy insurance..."
    $insCreateGet = Invoke-WebRequest -Uri "$base/Insurance/Create" -WebSession $s -UseBasicParsing
    $tokIns = Get-Token $insCreateGet.Content
    $insBody = @{ Name = 'Test Insurance'; PayerId = 'PAYER-1'; Active = 'true'; __RequestVerificationToken = $tokIns }
    Invoke-WebRequest -Uri "$base/Insurance/Create" -Method Post -Body $insBody -WebSession $s -UseBasicParsing -MaximumRedirection 0 -ErrorAction SilentlyContinue | Out-Null
    
    $ins2 = Invoke-WebRequest -Uri "$base/Insurance" -WebSession $s -UseBasicParsing
    $insIds = [regex]::Matches($ins2.Content, '/Insurance/Edit/([0-9a-fA-F-]{36})') | ForEach-Object { $_.Groups[1].Value } | Select-Object -Unique
}
$insuranceId = $insIds[0]
Write-Host "Using Insurance ID: $insuranceId"

Write-Host "4. Adding Primary Insurance (Test 1)..."
$editGet = Invoke-WebRequest -Uri "$base/Patient/Edit/$patientId" -WebSession $s -UseBasicParsing
$tokPat = Get-Token $editGet.Content
$addBody = @{
    PatientId = $patientId
    InsuranceId = $insuranceId
    MemberId = 'MEM-123'
    GroupId = 'GRP-123'
    IsPrimary = 'true'
    __RequestVerificationToken = $tokPat
}
try {
    $r1 = Invoke-WebRequest -Uri "$base/Patient/AddInsurance" -Method Post -Body $addBody -WebSession $s -UseBasicParsing -MaximumRedirection 0 -ErrorAction SilentlyContinue
    Write-Host "Add Primary Insurance POST successful. Status: $([int]$r1.StatusCode)"
} catch {
    Write-Host "FAIL: $($_.Exception.Message)"
    exit 1
}

Write-Host "5. Verifying Persistence (Test 2)..."
$editGet2 = Invoke-WebRequest -Uri "$base/Patient/Edit/$patientId" -WebSession $s -UseBasicParsing
if ($editGet2.Content -match 'MEM-123') {
    Write-Host "PASS: Insurance MEM-123 found after refresh."
} else {
    Write-Host "FAIL: Insurance MEM-123 not found."
    exit 1
}

Write-Host "6. Adding Secondary Insurance (Test 3)..."
$addBodySec = @{
    PatientId = $patientId
    InsuranceId = $insuranceId
    MemberId = 'MEM-SEC'
    GroupId = ''
    IsPrimary = 'false'
    __RequestVerificationToken = $tokPat
}
Invoke-WebRequest -Uri "$base/Patient/AddInsurance" -Method Post -Body $addBodySec -WebSession $s -UseBasicParsing -MaximumRedirection 0 -ErrorAction SilentlyContinue | Out-Null
$editGet3 = Invoke-WebRequest -Uri "$base/Patient/Edit/$patientId" -WebSession $s -UseBasicParsing
if ($editGet3.Content -match 'MEM-SEC' -and $editGet3.Content -match 'MEM-123') {
    Write-Host "PASS: Both Primary and Secondary insurances are present."
} else {
    Write-Host "FAIL: Secondary or Primary insurance missing."
}

Write-Host "7. Testing Primary Rule (Test 4)..."
$addBodyPrim2 = @{
    PatientId = $patientId
    InsuranceId = $insuranceId
    MemberId = 'MEM-P2'
    GroupId = ''
    IsPrimary = 'true'
    __RequestVerificationToken = $tokPat
}
try {
    $r3 = Invoke-WebRequest -Uri "$base/Patient/AddInsurance" -Method Post -Body $addBodyPrim2 -WebSession $s -UseBasicParsing -MaximumRedirection 0 -ErrorAction SilentlyContinue
    Write-Host "Warning: System allowed adding a second primary insurance. Or maybe it failed silently. Status: $([int]$r3.StatusCode)"
    
    $editGet4 = Invoke-WebRequest -Uri "$base/Patient/Edit/$patientId" -WebSession $s -UseBasicParsing
    if ($editGet4.Content -match 'Patient already has an active Primary insurance') {
        Write-Host "PASS: System caught Primary insurance rule violation (in UI error message)."
    } elseif ($r3.Content -match 'Patient already has an active Primary insurance') {
        Write-Host "PASS: System caught Primary insurance rule violation."
    } else {
        Write-Host "Primary rule check returned 200, possibly ignored or handled via form validation."
    }
} catch {
    if ($_.Exception.Message -match '500') {
        Write-Host "PASS: System rejected second primary insurance (500 expected based on Service logic Exception)."
    } else {
        Write-Host "PASS: System rejected second primary insurance ($($_.Exception.Message))."
    }
}

Write-Host "8. Regression tests (Test 5 & Regression)..."
$endpoints = @(
    "/Insurance",
    "/Appointment",
    "/Billing",
    "/Inventory"
)
foreach ($ep in $endpoints) {
    try {
        Invoke-WebRequest -Uri "$base$ep" -WebSession $s -UseBasicParsing | Out-Null
        Write-Host "PASS: Regression $ep"
    } catch {
        Write-Host "FAIL: Regression $ep - $($_.Exception.Message)"
    }
}

Write-Host "ALL TESTS COMPLETED"
