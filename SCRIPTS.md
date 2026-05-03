# Scripts de Utilidad

Scripts PowerShell para facilitar el desarrollo y ejecución del proyecto.

## Requisitos

Windows 10/11 con PowerShell. Si tienes error de permisos de ejecución:

```powershell
Set-ExecutionPolicy -ExecutionPolicy Bypass -Scope CurrentUser
```

## Scripts Disponibles

### 1. build.ps1 - Compilar el Proyecto

```powershell
.\build.ps1
```

**Qué hace:**
- Limpia builds anteriores
- Restaura dependencias NuGet
- Compila el proyecto completo
- Muestra resultado (✅ exitoso o ❌ falló)

**Output esperado:**
```
Build exitoso!

Próximos pasos:
1. Ejecutar servidor: powershell .\run-server.ps1
2. Ejecutar cliente: powershell .\run-client.ps1
```

### 2. run-server.ps1 - Ejecutar Servidor

```powershell
.\run-server.ps1
```

**Qué hace:**
- Navega a `Imperium AO 2\Server`
- Ejecuta `dotnet run`
- Inicia el servidor en puerto 7666
- Crea la base de datos automáticamente

**Output esperado:**
```
[HH:MM:SS INF] Starting ImperiumAO Server
[HH:MM:SS INF] Database migrations applied successfully
[HH:MM:SS INF] Starting TCP Network Server on port 7666
[HH:MM:SS INF] Server ready, waiting for connections
```

**Para detener:** Presiona `Ctrl+C`

### 3. run-client.ps1 - Ejecutar Cliente

```powershell
.\run-client.ps1
```

**Qué hace:**
- Navega a `Imperium AO 2\Client`
- Ejecuta `dotnet run`
- Abre ventana de juego (1024x768)
- Se conecta a localhost:7666

**Requisitos:**
- Servidor debe estar ejecutándose en otra terminal

## Flujo Típico de Desarrollo

### Primera Ejecución

```powershell
# Terminal 1 - Compilar
.\build.ps1

# Terminal 2 - Ejecutar servidor
.\run-server.ps1

# Terminal 3 - Ejecutar cliente
.\run-client.ps1
```

### Cambios de Código

Después de hacer cambios en el código:

```powershell
# Terminal actual
Ctrl+C  # Detener

# Opción A: Recompilar y ejecutar
.\build.ps1
.\run-server.ps1  # o .\run-client.ps1

# Opción B: Solo recompilar (más rápido)
cd "Imperium AO 2"
dotnet build
dotnet run
```

## Workflow Recomendado con Visual Studio

Si usas Visual Studio 2022:

1. Abre `Imperium AO 2\ImperiumAO.CSharp.sln`
2. En Solution Explorer, configura como Startup Project:
   - Para debugear servidor: Click derecho `ImperiumAO.Server` → Set as Startup Project
   - Para debugear cliente: Click derecho `ImperiumAO.Client` → Set as Startup Project
3. Presiona `F5` para ejecutar con debugger

### Debugging con Breakpoints

```csharp
// En cualquier archivo .cs
public void MyMethod()
{
    int x = 5;
    int y = 10;  // ← Click en el margen izquierdo para agregar breakpoint
    int result = x + y;
}
```

- Presiona `F5` para iniciar con debugger
- Presiona `F9` para toggle breakpoint
- Presiona `F10` para step over
- Presiona `F11` para step into

## Comandos Manuales (Sin Scripts)

Si prefieres no usar scripts:

### Compilar
```bash
cd "Imperium AO 2"
dotnet build
```

### Ejecutar Servidor
```bash
cd "Imperium AO 2\Server"
dotnet run
```

### Ejecutar Cliente
```bash
cd "Imperium AO 2\Client"
dotnet run
```

### Ejecutar Pruebas (cuando existan)
```bash
cd "Imperium AO 2"
dotnet test
```

### Limpiar
```bash
cd "Imperium AO 2"
dotnet clean
```

## Troubleshooting

### Error: "The term 'build.ps1' is not recognized"

**Solución:** Ejecuta desde la raíz del proyecto:
```powershell
cd C:\ruta\a\imperium-ao
.\build.ps1
```

### Error: "PowerShell execution policy prevents running scripts"

**Solución:**
```powershell
Set-ExecutionPolicy -ExecutionPolicy Bypass -Scope CurrentUser -Force
.\build.ps1
```

### Build falla con "dotnet not found"

**Solución:** Instala .NET 8 SDK desde https://dotnet.microsoft.com/download/dotnet/8.0

### Cliente no conecta al servidor

**Checklist:**
1. ✅ Servidor está ejecutándose (`.\run-server.ps1`)
2. ✅ No hay error en consola del servidor
3. ✅ Puerto 7666 está libre: `netstat -ano | findstr 7666`
4. ✅ Firewall de Windows permite .NET

## Automatización Avanzada

### Crear alias permanente

En tu perfil de PowerShell (`$PROFILE`):

```powershell
# Abrir editor
notepad $PROFILE

# Agregar estas líneas
function Build { & "$PSScriptRoot\build.ps1" }
function RunServer { & "$PSScriptRoot\run-server.ps1" }
function RunClient { & "$PSScriptRoot\run-client.ps1" }

# Guardar y recargar perfil
& $PROFILE
```

Luego puedes usar directamente:
```powershell
Build
RunServer
RunClient
```

### Ejecutar Servidor y Cliente simultáneamente

Crear `run-all.ps1`:

```powershell
Write-Host "Iniciando servidor y cliente..." -ForegroundColor Cyan

# Abre servidor en nueva ventana
Start-Process powershell -ArgumentList "-NoExit", "-Command", "& '.\run-server.ps1'"

# Espera un segundo para que el servidor inicie
Start-Sleep -Seconds 1

# Abre cliente en nueva ventana
Start-Process powershell -ArgumentList "-NoExit", "-Command", "& '.\run-client.ps1'"

Write-Host "Servidor y cliente iniciados en ventanas separadas" -ForegroundColor Green
```

Luego:
```powershell
.\run-all.ps1
```

## Ver Logs en Tiempo Real

```powershell
# Monitorear logs del servidor
Get-Content -Path ".\Imperium AO 2\Server\logs\imperiumao-*.txt" -Wait

# O buscar errores específicos
Get-Content -Path ".\Imperium AO 2\Server\logs\imperiumao-*.txt" | Select-String "Error"
```

---

**¡Desarrollo ágil con scripts!** 🚀
