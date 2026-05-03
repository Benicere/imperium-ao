# Script para ejecutar el cliente Imperium AO

Write-Host "======================================" -ForegroundColor Cyan
Write-Host "  IMPERIUM AO - CLIENTE" -ForegroundColor Green
Write-Host "======================================" -ForegroundColor Cyan
Write-Host ""

$clientPath = ".\Imperium AO 2\Client"

if (-not (Test-Path $clientPath)) {
    Write-Host "Error: No se encontró la ruta del cliente: $clientPath" -ForegroundColor Red
    exit 1
}

Write-Host "Iniciando cliente..." -ForegroundColor Yellow
Write-Host "Resolución: 1024x768" -ForegroundColor Gray
Write-Host "Servidor: localhost:7666" -ForegroundColor Gray
Write-Host ""

Set-Location $clientPath
dotnet run

Write-Host ""
Write-Host "Cliente cerrado." -ForegroundColor Yellow
