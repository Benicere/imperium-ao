# ImperiumAO - Versión C# (.NET 8)

Conversión moderna del servidor y cliente ImperiumAO desde Visual Basic 6 a C# con .NET 8.

## Estructura del Proyecto

```
ImperiumAO.CSharp.sln
├── Common/
│   ├── Database/
│   │   └── DatabaseConfig.cs
│   ├── Entities/
│   │   ├── Character.cs
│   │   ├── Player.cs
│   │   └── Npc.cs
│   └── Network/
│       └── ByteBuffer.cs
├── Server/
│   ├── Core/
│   │   ├── GameServer.cs
│   │   ├── WorldManager.cs
│   │   ├── PlayerManager.cs
│   │   ├── NpcManager.cs
│   │   ├── CombatSystem.cs
│   │   └── SkillSystem.cs
│   ├── Program.cs
│   ├── ServiceConfiguration.cs
│   └── appsettings.json
└── Client/
    └── (Windows Forms UI - En conversión)
```

## Requisitos

- .NET 8 SDK o superior
- Visual Studio 2022 o VS Code
- MySQL 5.7+
- Base de datos configurada según `Fixtures/`

## Instalación y Configuración

### 1. Instalar .NET 8

```bash
# Verificar versión instalada
dotnet --version

# Si no está instalado, descargar desde:
# https://dotnet.microsoft.com/download/dotnet/8.0
```

### 2. Configurar Base de Datos

```bash
# Crear bases de datos MySQL
mysql -u root -p < "../Imperium AO 1/Fixtures/cuentas.sql"
mysql -u root -p < "../Imperium AO 1/Fixtures/personajes.sql"
```

### 3. Configurar appsettings.json

Editar `Server/appsettings.json` con tus credenciales MySQL:

```json
{
  "Database": {
    "CharacterConnectionString": "Server=localhost;User=root;Password=tuPassword;Database=imperium_characters",
    "AccountConnectionString": "Server=localhost;User=root;Password=tuPassword;Database=imperium_accounts"
  }
}
```

### 4. Restaurar dependencias y compilar

```bash
cd Server
dotnet restore
dotnet build
```

## Ejecución

### Iniciar el servidor

```bash
dotnet run --configuration Release
```

El servidor iniciará en el puerto configurado (por defecto 7666).

### Iniciar el cliente

```bash
cd ../Client
dotnet run
```

## Conversión de VB6 a C#

Se está convirtiendo progresivamente desde Visual Basic 6. Revisar:
- [CONVERSION_GUIDE.md](../CONVERSION_GUIDE.md) - Guía detallada de mapeo VB6↔C#
- [convert-vb6-to-csharp.ps1](../convert-vb6-to-csharp.ps1) - Script de asistencia

### Módulos Convertidos

✅ **Common**
- ByteBuffer.cs (Network serialization)
- Character entities (Player, NPC)
- Database configuration

✅ **Server Core**
- GameServer
- WorldManager
- PlayerManager
- NpcManager
- CombatSystem
- SkillSystem

⏳ **Pendientes**
- Sistema de Combat avanzado
- Sistema de Skills completo
- Sistema de Clanes
- Sistema de Items
- Base de datos (DAL/Repository)
- Networking (TCP/UDP)
- AI para NPCs

## Arquitectura

### Dependency Injection

El proyecto usa .NET's built-in DI container en `ServiceConfiguration.cs`:

```csharp
services.AddSingleton<IWorldManager, WorldManager>();
services.AddSingleton<IPlayerManager, PlayerManager>();
```

### Logging

Usa Serilog para logging estructurado:

```csharp
_logger.LogInformation("Evento importante");
_logger.LogError(ex, "Error ocurrido");
```

Los logs se escriben en:
- Console (desarrollo)
- Archivos en carpeta `logs/` (automático)

### Async/Await

El código C# usa patrones async para operaciones I/O:

```csharp
await _playerManager.SavePlayerAsync(player);
await _worldManager.InitializeAsync(cancellationToken);
```

## Testing

```bash
# Ejecutar tests
dotnet test

# Con cobertura
dotnet test /p:CollectCoverage=true
```

## Contribución

1. Revisar [CONVERSION_GUIDE.md](../CONVERSION_GUIDE.md)
2. Crear rama para la conversión del módulo
3. Convertir módulo de VB6
4. Agregar/actualizar tests
5. Crear PR con descripción de cambios

## Diferencias Clave respecto a VB6

| VB6 | C# |
|-----|-----|
| Global variables | Dependency Injection |
| On Error GoTo | try/catch + Logging |
| API Calls (Declare) | P/Invoke o bibliotecas .NET |
| ODBC Connections | MySqlConnector |
| File I/O directo | File APIs modernas |
| MsgBox | Logging (Serilog) |

## Performance

- **Multithreading**: .NET maneja mejor concurrencia
- **Memory**: Garbage collection automático
- **Network**: Async sockets para mejor escalabilidad
- **Database**: Connection pooling automático

## Debugging

### Visual Studio

1. Abrir `ImperiumAO.CSharp.sln`
2. Establecer breakpoints
3. Presionar F5 para debug

### VS Code + Debugger for C#

```bash
dotnet watch run --configuration Debug
```

## Documentación Oficial

- [Learn C#](https://learn.microsoft.com/en-us/dotnet/csharp/)
- [Logging in .NET](https://learn.microsoft.com/en-us/dotnet/core/extensions/logging)
- [Dependency Injection](https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection)
- [AsyncAwait Pattern](https://learn.microsoft.com/en-us/archive/msdn-magazine/2013/march/async-await-best-practices-in-asynchronous-programming)

## Licencia

Mismo que el proyecto original.

## Soporte

Revisar el repositorio original y documentación de conversión para issues específicos.
