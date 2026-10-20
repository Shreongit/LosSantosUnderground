$ErrorActionPreference = "Stop"

$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$project = Join-Path $root "src\LosSantosUnderground.csproj"
$output = Join-Path $root "build\output\net48"
$lemonUi = Join-Path $root "src\Dependencies\LemonUI.SHVDN3.dll"

Write-Host "Building Los Santos Underground..." -ForegroundColor Cyan
dotnet build $project --configuration Release

if ($LASTEXITCODE -ne 0) {
    throw "Build failed. See the compiler output above."
}

$dll = Join-Path $output "LosSantosUnderground.dll"
if (-not (Test-Path $dll)) {
    throw "Build completed but expected output was not found: $dll"
}

if (Test-Path $lemonUi) {
    Copy-Item $lemonUi (Join-Path $output "LemonUI.SHVDN3.dll") -Force
}

Write-Host ""
Write-Host "Build successful." -ForegroundColor Green
Write-Host "Install the contents of this folder into GTA V's scripts directory:"
Write-Host $output
