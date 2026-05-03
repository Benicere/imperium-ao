# Script para ejecutar el servidor Imperium AO

Write-Host "======================================" -ForegroundColor Cyan
Write-Host "  IMPERIUM AO - SERVIDOR" -ForegroundColor Green
Write-Host "======================================" -ForegroundColor Cyan
Write-Host ""

$serverPath = ".\Imperium AO 2\Server"

if (-not (Test-Path $serverPath)) {
    Write-Host "Error: No se encontró la ruta del servidor: $serverPath" -ForegroundColor Red
    exit 1
}

Write-Host "Iniciando servidor..." -ForegroundColor Yellow
Write-Host "Puerto: 7666" -ForegroundColor Gray
Write-Host "Logs: ./Imperium AO 2/Server/logs/" -ForegroundColor Gray
Write-Host ""

Set-Location $serverPath
dotnet run

Write-Host ""
Write-Host "Servidor detenido." -ForegroundColor Yellow
