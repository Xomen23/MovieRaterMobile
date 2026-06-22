# Pokreni Spring Boot backend sa JDK 21 (Maven ne koristi JAVA_HOME iz Windows-a u starom terminalu).
$jdk21 = "C:\Program Files\Eclipse Adoptium\jdk-21.0.11.10-hotspot"
if (-not (Test-Path $jdk21)) {
    Write-Error "JDK 21 nije pronadjen na: $jdk21`nProveri putanju (ipconfig / where java)."
    exit 1
}

$env:JAVA_HOME = $jdk21
$env:Path = "$env:JAVA_HOME\bin;" + ($env:Path -replace [regex]::Escape("C:\Program Files\Java\jdk8\bin;"), "")

Write-Host "JAVA_HOME=$env:JAVA_HOME"
java -version 2>&1 | Select-Object -First 1

Set-Location $PSScriptRoot
.\mvnw.cmd spring-boot:run
