param(
    [string]$SourceModule = "",
    [string]$TargetNamespace = "ImperiumAO.Server"
)

function Get-VB6Modules {
    param(
        [string]$Path = "Imperium AO 1\Server\Codigo"
    )

    $modules = @()

    Get-ChildItem -Path $Path -Filter "*.bas" | ForEach-Object {
        $modules += @{
            Name = $_.BaseName
            Type = "Standard Module"
            Path = $_.FullName
        }
    }

    Get-ChildItem -Path $Path -Filter "*.cls" | ForEach-Object {
        $modules += @{
            Name = $_.BaseName
            Type = "Class Module"
            Path = $_.FullName
        }
    }

    return $modules
}

function Get-ModuleDependencies {
    param(
        [string]$ModulePath
    )

    $content = Get-Content $ModulePath -Raw
    $dependencies = @()

    # Buscar Imports/Uses
    if ($content -match 'Imports\s+(\S+)') {
        $dependencies += $matches[1]
    }

    return $dependencies
}

function New-CSharpClass {
    param(
        [string]$ClassName,
        [string]$Namespace,
        [string]$OutputPath
    )

    $classTemplate = @"
namespace $Namespace;

public class $ClassName
{
    // TODO: Implementar lógica convertida de VB6
}
"@

    $classTemplate | Set-Content -Path $OutputPath
}

function Show-ConversionStats {
    param(
        [string]$SourcePath = "Imperium AO 1"
    )

    $modules = Get-VB6Modules -Path "$SourcePath\Server\Codigo"
    $clientModules = Get-VB6Modules -Path "$SourcePath\Cliente\CODIGO"

    Write-Host "=== VB6 Conversion Statistics ===" -ForegroundColor Cyan
    Write-Host "Server Modules: $($modules.Count)"
    Write-Host "Client Modules: $($clientModules.Count)"
    Write-Host "Total: $($modules.Count + $clientModules.Count)"
    Write-Host ""

    Write-Host "=== Server Modules ===" -ForegroundColor Yellow
    $modules | ForEach-Object { Write-Host "  [$($_.Type)] $($_.Name)" }
}

# Menú principal
if (-not $SourceModule) {
    Write-Host "=== ImperiumAO VB6 to C# Conversion Tool ===" -ForegroundColor Green
    Write-Host ""
    Write-Host "1. Show conversion statistics"
    Write-Host "2. List all VB6 modules"
    Write-Host "3. Get module dependencies"
    Write-Host "4. Create C# class template"
    Write-Host "5. Exit"
    Write-Host ""

    $choice = Read-Host "Select option"

    switch ($choice) {
        "1" { Show-ConversionStats }
        "2" { Get-VB6Modules | Format-Table Name, Type, Path }
        "3" {
            $module = Read-Host "Enter module name"
            $path = "Imperium AO 1\Server\Codigo\$module.cls"
            if (Test-Path $path) {
                Get-ModuleDependencies -ModulePath $path
            } else {
                Write-Host "Module not found: $module" -ForegroundColor Red
            }
        }
        "4" {
            $className = Read-Host "Enter C# class name"
            $outputPath = "Imperium AO 2\Server\$className.cs"
            New-CSharpClass -ClassName $className -Namespace $TargetNamespace -OutputPath $outputPath
            Write-Host "Created: $outputPath" -ForegroundColor Green
        }
    }
}
