# Script para compilar el proyecto Imperium AO

Write-Host "======================================" -ForegroundColor Cyan
Write-Host "  IMPERIUM AO - BUILD" -ForegroundColor Green
Write-Host "======================================" -ForegroundColor Cyan
Write-Host ""

$projectPath = ".\Imperium AO 2"

if (-not (Test-Path $projectPath)) {
    Write-Host "Error: No se encontró la ruta del proyecto: $projectPath" -ForegroundColor Red
    exit 1
}

Write-Host "Limpiando builds anteriores..." -ForegroundColor Yellow
Set-Location $projectPath
dotnet clean 2>&1 | Out-Null

Write-Host "Restaurando dependencias..." -ForegroundColor Yellow
dotnet restore

Write-Host ""
Write-Host "Compilando proyecto..." -ForegroundColor Yellow
$buildResult = dotnet build

if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Host "✅ Build exitoso!" -ForegroundColor Green
    Write-Host ""
    Write-Host "Próximos pasos:" -ForegroundColor Cyan
    Write-Host "1. Ejecutar servidor: powershell -ExecutionPolicy Bypass -File run-server.ps1" -ForegroundColor Gray
    Write-Host "2. Ejecutar cliente: powershell -ExecutionPolicy Bypass -File run-client.ps1" -ForegroundColor Gray
} else {
    Write-Host ""
    Write-Host "❌ Build falló. Revisa los errores arriba." -ForegroundColor Red
    exit 1
}
