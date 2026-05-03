# ⚡ Quick Start - ImperiumAO C#

## 5 Minutos para Empezar

### 1. Instalar .NET 8

```powershell
# Verificar si está instalado
dotnet --version

# Si no está, descargar desde:
# https://dotnet.microsoft.com/download/dotnet/8.0
```

### 2. Abrir el Proyecto

#### Opción A: Visual Studio 2022
```powershell
# Doble-click en:
"Imperium AO 2\ImperiumAO.CSharp.sln"
```

#### Opción B: VS Code
```powershell
cd "Imperium AO 2"
code .
```

#### Opción C: Línea de comandos
```powershell
cd "Imperium AO 2\Server"
dotnet restore
```

### 3. Compilar

```powershell
cd "Imperium AO 2\Server"
dotnet build
```

### 4. Ejecutar Servidor

```powershell
dotnet run --configuration Release
```

**Debería mostrar:**
```
info: ImperiumAO.Server.GameServer[0]
      Starting ImperiumAO Server
info: ImperiumAO.Server.WorldManager[0]
      World Manager initializing
info: ImperiumAO.Server[0]
      Game server is running. Press Ctrl+C to stop.
```

### 5. Detener Servidor

```
Presionar: Ctrl+C
```

---

## 📁 Archivos Importantes

| Archivo | Propósito | 
|---------|-----------|
| `Program.cs` | Punto de entrada |
| `ServiceConfiguration.cs` | Configuración de DI |
| `appsettings.json` | BD, logging |
| `GameServer.cs` | Control principal |
| `Core/*Manager.cs` | Gestores de sistemas |

---

## 🔧 Configurar Base de Datos

### 1. Editar appsettings.json

```json
{
  "Database": {
    "CharacterConnectionString": "Server=localhost;User=root;Password=TU_PASSWORD;Database=imperium_characters",
    "AccountConnectionString": "Server=localhost;User=root;Password=TU_PASSWORD;Database=imperium_accounts"
  }
}
```

### 2. Crear BD en MySQL

```bash
# Abrir MySQL
mysql -u root -p

# Crear bases de datos
CREATE DATABASE imperium_characters CHARACTER SET utf8mb4;
CREATE DATABASE imperium_accounts CHARACTER SET utf8mb4;

# Importar fixtures
mysql -u root -p imperium_characters < "Fixtures\personajes.sql"
mysql -u root -p imperium_accounts < "Fixtures\cuentas.sql"
```

---

## 💻 Ejemplos de Código

### Usar PlayerManager (Dependency Injection)

```csharp
// En Program.cs, ya está inyectado
var playerManager = serviceProvider.GetRequiredService<IPlayerManager>();

// Cargar jugador
var player = await playerManager.LoadPlayerAsync(1);

// Guardar jugador
player.Health = 100;
await playerManager.SavePlayerAsync(player);
```

### Usar CombatSystem

```csharp
var combatSystem = serviceProvider.GetRequiredService<ICombatSystem>();

// Atacar
var attacker = new Player { Name = "Hero", Health = 100 };
var target = new Player { Name = "Enemy", Health = 50 };

combatSystem.Attack(attacker, target);
```

### Crear ByteBuffer para Red

```csharp
using ImperiumAO.Common.Network;

var buffer = new ByteBuffer();

// Escribir
buffer.PutByte(1);
buffer.PutInteger(100);
buffer.PutString("Hello");

// Leer
buffer.InitializeReader(buffer.GetBytes());
byte type = buffer.GetByte();      // 1
short value = buffer.GetInteger(); // 100
string message = buffer.GetString(); // "Hello"
```

---

## 📊 Estructura del Código

```
Program.cs (Main)
    ↓
ServiceConfiguration (Configura DI)
    ↓
GameServer (Gestor principal)
    ├─→ WorldManager (Mundo)
    ├─→ PlayerManager (Jugadores)
    ├─→ NpcManager (NPCs)
    ├─→ CombatSystem (Combate)
    └─→ SkillSystem (Habilidades)
```

---

## 🐛 Troubleshooting

### Error: "dotnet command not found"
```powershell
# Instalar .NET 8 SDK
# https://dotnet.microsoft.com/download/dotnet/8.0
```

### Error: "Cannot connect to database"
```
Verificar:
- MySQL está corriendo
- Usuario/password en appsettings.json son correctos
- Bases de datos existen
- Fixtures fueron importados
```

### Error: "Class not found"
```
Ejecutar:
dotnet clean
dotnet restore
dotnet build
```

### Error: "Port already in use"
```
Cambiar puerto en appsettings.json:
"Server": {
  "Port": 7667  // Cambiar de 7666
}
```

---

## 📚 Documentación Completa

Para detalles completos, ver:
- [README.md](Imperium%20AO%202/README.md) - Instalación y configuración
- [CONVERSION_GUIDE.md](CONVERSION_GUIDE.md) - Cómo convertir VB6
- [PROJECT_STATUS.md](PROJECT_STATUS.md) - Estado y milestones
- [ESTRUCTURA.txt](Imperium%20AO%202/ESTRUCTURA.txt) - Árbol del proyecto

---

## 🎯 Pasos Siguientes

### Desarrollador
1. Familiarizarse con estructura
2. Leer CONVERSION_GUIDE.md
3. Convertir módulo VB6 siguiente
4. Implementar tests

### Usuario
1. Configurar BD
2. Ejecutar servidor
3. Conectar cliente
4. Probar

---

## ⌨️ Comandos Útiles

```powershell
# Compilar
dotnet build

# Ejecutar
dotnet run

# Tests
dotnet test

# Watch mode (auto-reload)
dotnet watch run

# Limpiar build
dotnet clean

# Restore packages
dotnet restore

# Ver estructura proyectos
dotnet sln list

# Crear nuevo archivo
dotnet new classlib -n MyLib
```

---

## 🔐 Seguridad

**IMPORTANTE**: Nunca commit `appsettings.json` con passwords reales.

```bash
# Usar secrets locales en desarrollo
dotnet user-secrets set "Database:CharacterConnectionString" "..."

# O usar environment variables
$env:ConnectionStrings__Characters = "..."
```

---

## 📞 Ayuda Rápida

| Problema | Solución |
|----------|----------|
| ¿Cómo cambio el puerto? | Ver `appsettings.json` → `Server.Port` |
| ¿Cómo agrego logging? | Ver `Serilog.txt` en carpeta `logs/` |
| ¿Cómo conecto a BD? | Ver `Program.cs` → `ServiceConfiguration.cs` |
| ¿Cómo creo un Manager? | Crear interfaz + implementación, luego agregar en `ServiceConfiguration.cs` |
| ¿Cómo testeo? | Crear `XUnitProject`, agregar referencia a Common/Server, usar `[Fact]` |

---

**¡Ahora estás listo para empezar a desarrollar! 🚀**

Ver [CONVERSION_GUIDE.md](CONVERSION_GUIDE.md) para convertir módulos VB6.
