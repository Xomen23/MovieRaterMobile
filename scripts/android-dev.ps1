# Pokreni kao Administrator za firewall, ili samo za adb reverse (bez admin-a).
$adb = "$env:LOCALAPPDATA\Android\Sdk\platform-tools\adb.exe"
if (-not (Test-Path $adb)) {
    Write-Error "adb nije pronadjen: $adb"
    exit 1
}

& $adb reverse tcp:8080 tcp:8080
Write-Host "adb reverse postavljen (USB fallback u app-u)."
Write-Host "App automatski proba Wi-Fi ($($env:COMPUTERNAME)) pa USB 127.0.0.1:8080."

$isAdmin = ([Security.Principal.WindowsPrincipal][Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)
if ($isAdmin) {
    netsh advfirewall firewall add rule name="MovieRater Backend TCP 8080" dir=in action=allow protocol=TCP localport=8080 profile=private,public,domain 2>$null
    Write-Host "Firewall pravilo za port 8080 dodato (za Wi-Fi / Lan mod)."
} else {
    Write-Host "Za Wi-Fi mod pokreni ovaj skript ponovo kao Administrator da otvoris port 8080."
}
