# Plan: ImperiumAO VB6 → C# .NET 8 — Completamente Jugable

## Contexto
El usuario quiere que el juego MMORPG **ImperiumAO** (basado en Argentum Online, codebase VB6) sea convertido completamente a C# y sea jugable end-to-end. El código original está en `Imperium AO 1/` y el proyecto C# en `Imperium AO 2/`. Actualmente el proyecto C# solo tiene un skeleton (DI, logging, entidades base) sin red, sin DB real, y sin cliente. Hay que implementar todo para que sea jugable: login → selección de personaje → mapa → movimiento → chat → combate → guardar.

## Información Crítica del Protocolo VB6

### PacketIDs (contados desde VB6 Protocol.bas)

**ServerPacketID** (primeros relevantes para MVP):
- Logged=1, RemoveDialogs=2, RemoveCharDialog=3, NavigateToggle=4, Disconnect=5
- UpdateSta=11, UpdateMana=12, UpdateHP=13, UpdateGold=14, UpdateExp=15
- ChangeMap=16, PosUpdate=17, ChatOverHead=18, ConsoleMsg=19
- UserIndexInServer=22, UserCharIndexInServer=23
- CharacterCreate=24, CharacterRemove=25, CharacterMove=27
- errorMsg=58 (contar exactamente en enum)
- EnviarPJUserAccount=126 (contar exactamente — posición real en enum)

**ClientPacketID** (relevantes):
- LoginExistingChar=1, LoginNewChar=2, Talk=3, Yell=4, Whisper=5, Walk=6
- UseItem=7, RequestPositionUpdate=8, Attack=9, PickUp=10
- Quit=62 (contar desde SafeToggle=11)
- Ping=107 (contar: Online=61, Quit=62, GuildLeave=63... hasta Ping)
- LoginExistingAccount=131 (contar desde LoginExistingChar=1)

**IMPORTANTE**: Los valores exactos del enum VB6 son implícitos. Hay que contarlos cuidadosamente desde `Imperium AO 1/Server/Codigo/Protocol.bas`.

### Base de Datos Real

Existen en `Fixtures/Base de datos/`:
- `iaccuentas.sql` → BD `iaccuentas`, tabla `cuentas` (id, username, email, password, salt, **status=0 activo**, last_ip, date_created, date_last_login)
- `iacuser.sql` → BD `iacuser`, tabla `personaje` (id, cuenta_id, deleted, name, level, exp, genre_id, race_id, class_id, home_id, gold, pos_map, pos_x, pos_y, body_id, head_id, weapon_id, helmet_id, shield_id, heading, min_hp, max_hp, min_man, max_man, min_sta, max_sta, rep_average, is_dead, etc.)

### Bugs Confirmados en Código Existente
1. `Common/Entities/Player.cs:21` — `public bool IsAlive => Health > 0;` oculta `Character.IsAlive()` → **eliminar**
2. `Common/Entities/Npc.cs:8` — misma situación → **eliminar**
3. `Common/Network/ByteBuffer.cs:195` — `IsEof()` usa `_data.Length` en vez de `_lastPos` → **fix**
4. `Server/Core/WorldManager.cs:20` — `_ = UpdateLoopAsync()` fire-and-forget sin manejo de excepciones → **guardar task**
5. `Server/Core/CombatSystem.cs` — `new Random()` por llamada → **usar `Random.Shared`**

---

## Estructura de Archivos a Crear

### FASE 0 — Correcciones (archivos existentes)
```
Common/Entities/Player.cs              → eliminar: public bool IsAlive => Health > 0;
Common/Entities/Npc.cs                 → eliminar: public bool IsAlive => Health > 0;
Common/Network/ByteBuffer.cs           → fix IsEof(): _currentPos > _lastPos (no _data.Length)
Server/Core/WorldManager.cs            → guardar _updateTask, esperar en shutdown
Server/Core/CombatSystem.cs            → Random.Shared en vez de new Random()
```

### FASE 1 — Protocolo y Red
```
Common/Network/PacketIds.cs            (NUEVO - enums ServerPacketId + ClientPacketId)
Server/Network/ServerOptions.cs        (NUEVO - Port: 7666, MaxConnections: 500)
Server/Network/IGameConnection.cs      (NUEVO)
Server/Network/GameConnection.cs       (NUEVO - TcpClient + System.IO.Pipelines)
Server/Network/TcpNetworkServer.cs     (NUEVO - IHostedService, TcpListener)
Server/Network/PacketWriter.cs         (NUEVO - Write* para ServerPacketIds)
Server/Handlers/IPacketHandler.cs      (NUEVO)
Server/Handlers/PacketDispatcher.cs    (NUEVO)
Server/Core/IConnectionManager.cs      (NUEVO)
Server/Core/ConnectionManager.cs       (NUEVO - ConcurrentDictionary<int, IGameConnection>)
```

### FASE 2 — Base de Datos
```
Common/Database/Models/Account.cs                  (NUEVO - record)
Common/Database/Models/CharacterSummary.cs        (NUEVO - record)
Common/Database/IAccountRepository.cs             (NUEVO)
Common/Database/AccountRepository.cs              (NUEVO - MySqlConnector, SHA256)
Common/Database/ICharacterRepository.cs           (NUEVO)
Common/Database/CharacterRepository.cs            (NUEVO - MySqlConnector)
```

### FASE 3 — Handlers de Auth
```
Server/Handlers/AccountHandlers/LoginAccountHandler.cs        (NUEVO)
Server/Handlers/AccountHandlers/SelectCharacterHandler.cs     (NUEVO)
Server/Handlers/AccountHandlers/NewCharacterHandler.cs        (NUEVO)
```

### FASE 4 — Mapa y Game Loop
```
Server/Map/MapData.cs                  (NUEVO)
Server/Map/IMapManager.cs              (NUEVO)
Server/Map/MapManager.cs               (NUEVO - mapa 100x100 en memoria para MVP)
```

### FASE 5 — Handlers de Juego
```
Server/Handlers/GameHandlers/WalkHandler.cs       (NUEVO)
Server/Handlers/GameHandlers/TalkHandler.cs       (NUEVO)
Server/Handlers/GameHandlers/AttackHandler.cs     (NUEVO)
Server/Handlers/GameHandlers/PingHandler.cs       (NUEVO)
Server/Handlers/GameHandlers/QuitHandler.cs       (NUEVO)
```

### FASE 6 — Cliente MonoGame
```
Client/ImperiumAO.Client.csproj                   (MODIFICAR - MonoGame 3.8.1)
Client/Program.cs                                 (NUEVO)
Client/ImperiumGame.cs                            (NUEVO - : Game)
Client/Network/GameClient.cs                      (NUEVO - TcpClient)
Client/Network/ClientPacketHandler.cs             (NUEVO)
Client/Screens/IScreen.cs                         (NUEVO)
Client/Screens/LoginScreen.cs                     (NUEVO)
Client/Screens/CharacterSelectScreen.cs           (NUEVO)
Client/Screens/GameScreen.cs                      (NUEVO)
Client/GameState/ClientGameState.cs               (NUEVO)
Client/GameState/CharacterRenderData.cs           (NUEVO)
Client/Rendering/TileRenderer.cs                  (NUEVO)
Client/Rendering/CharacterRenderer.cs             (NUEVO)
```

---

## Cambios en Archivos Existentes

### `Server/ServiceConfiguration.cs`
```csharp
public static IServiceCollection AddConfiguration(this IServiceCollection services, IConfiguration configuration)
{
    services.Configure<DatabaseConfig>(configuration.GetSection("Database"));
    services.Configure<ServerOptions>(configuration.GetSection("Server"));
    return services;
}

public static IServiceCollection AddGameServices(this IServiceCollection services)
{
    // Networking
    services.AddSingleton<IConnectionManager, ConnectionManager>();
    services.AddSingleton<IPacketDispatcher, PacketDispatcher>();
    services.AddHostedService<TcpNetworkServer>();
    
    // Repositories
    services.AddScoped<IAccountRepository, AccountRepository>();
    services.AddScoped<ICharacterRepository, CharacterRepository>();
    
    // Game systems
    services.AddSingleton<IWorldManager, WorldManager>();
    services.AddSingleton<IPlayerManager, PlayerManager>();
    services.AddSingleton<INpcManager, NpcManager>();
    services.AddSingleton<ICombatSystem, CombatSystem>();
    services.AddSingleton<ISkillSystem, SkillSystem>();
    services.AddSingleton<IMapManager, MapManager>();
    
    // Auto-registro de handlers
    var handlerTypes = typeof(ServiceConfiguration).Assembly
        .GetTypes()
        .Where(t => t.IsClass && !t.IsAbstract && typeof(IPacketHandler).IsAssignableFrom(t));
    
    foreach (var handlerType in handlerTypes)
        services.AddSingleton(typeof(IPacketHandler), handlerType);
    
    services.AddSingleton<GameServer>();
    return services;
}
```

### `Server/appsettings.json`
```json
{
  "Server": {
    "Port": 7666,
    "MaxConnections": 500
  },
  "Database": {
    "CharacterConnectionString": "Server=localhost;User=root;Password=;Database=iacuser;SslMode=None;",
    "AccountConnectionString": "Server=localhost;User=root;Password=;Database=iaccuentas;SslMode=None;"
  },
  // ... rest of logging config
}
```

### `.csproj` Files

**`Common/ImperiumAO.Common.csproj`**: Sin cambios (MySqlConnector ya está)

**`Server/ImperiumAO.Server.csproj`**: Agregar:
```xml
<PackageReference Include="Serilog.Sinks.Console" Version="5.0.0" />
<PackageReference Include="Serilog.Sinks.File" Version="5.0.0" />
<PackageReference Include="Serilog.Settings.Configuration" Version="8.0.0" />
<PackageReference Include="Microsoft.Extensions.Hosting" Version="8.0.0" />
<PackageReference Include="System.IO.Pipelines" Version="8.0.0" />
```

**`Client/ImperiumAO.Client.csproj`**: Cambiar a:
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFramework>net8.0-windows</TargetFramework>
    <LangVersion>latest</LangVersion>
    <Nullable>enable</Nullable>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="MonoGame.Framework.WindowsDX" Version="3.8.1.303" />
    <PackageReference Include="MonoGame.Content.Builder.Task" Version="3.8.1.303" />
    <PackageReference Include="Microsoft.Extensions.DependencyInjection" Version="8.0.0" />
    <PackageReference Include="Serilog" Version="3.1.1" />
  </ItemGroup>
  <ItemGroup>
    <ProjectReference Include="..\Common\ImperiumAO.Common.csproj" />
  </ItemGroup>
</Project>
```

---

## Detalles Técnicos Críticos

### Framing de Paquetes
El protocolo VB6 **NO tiene prefijo de longitud**. Cada paquete es: `[byte PacketID] [datos binarios]`. Los datos pueden ser:
- **Fijos**: bytes, ints, floats (tamaño conocido)
- **Variables**: strings con prefijo `int16` de longitud

**Estrategia C#**: En `GameConnection.ReadPipeAsync()`, peek el primer byte, consultar tabla de tamaños mínimos, esperar si no hay suficientes bytes.

### Password Hashing
```csharp
public static string HashPassword(string password, string salt)
{
    var input = Encoding.UTF8.GetBytes(password + salt);
    return Convert.ToHexString(SHA256.HashData(input)).ToLower();
}
```

**IMPORTANTE**: La BD tiene `status=0` como activo (no 1).

### Diferencias PosUpdate vs CharacterMove
- `PosUpdate`: enviar SOLO al jugador que se movió (confirmación)
- `CharacterMove`: enviar a TODOS en el área (broadcast)

---

## Orden de Implementación (27 pasos)

1. **FASE 0**: Corregir 5 bugs existentes
2. **FASE 1a**: `PacketIds.cs` (contar VB6)
3. **FASE 1b**: `IGameConnection` + `GameConnection` + `TcpNetworkServer`
4. **FASE 1c**: `IPacketHandler` + `PacketDispatcher` + `IConnectionManager` + `ConnectionManager`
5. **FASE 1d**: `PacketWriter`
6. **FASE 1e**: `ServiceConfiguration.cs` + `ServerOptions` + `appsettings.json`
7. ✅ **Checkpoint 1**: `dotnet run` en Server, acepta TCP en 7666
8. **FASE 2a**: `Account.cs` + `CharacterSummary.cs`
9. **FASE 2b**: `AccountRepository.cs` (GetByUsername + GetCharacters)
10. **FASE 2c**: `CharacterRepository.cs` (Load + Save)
11. **FASE 2d**: `PlayerManager.cs` actualizado
12. ✅ **Checkpoint 2**: Queries MySQL funcionan
13. **FASE 3a**: `LoginAccountHandler.cs`
14. **FASE 3b**: `PacketWriter` paquetes login
15. **FASE 3c**: `SelectCharacterHandler.cs`
16. **FASE 4a**: `MapData.cs` + `MapManager.cs`
17. ✅ **Checkpoint 3**: Login funciona, entra al mapa
18. **FASE 5a**: `WalkHandler` + `TalkHandler` + `PingHandler` + `QuitHandler`
19. **FASE 5b**: `AttackHandler`
20. ✅ **Checkpoint 4**: Servidor completo
21. **FASE 6a**: Cambiar `Client.csproj` a MonoGame
22. **FASE 6b**: `Program.cs` + `ImperiumGame.cs`
23. **FASE 6c**: `GameClient.cs` + `ClientPacketHandler.cs`
24. **FASE 6d**: `ClientGameState.cs`
25. **FASE 6e**: `LoginScreen` + `CharacterSelectScreen`
26. **FASE 6f**: `GameScreen` + Rendering
27. ✅ **JUGABLE**: Login → mapa → movimiento → chat → combate → guardar

---

## Verificación Final

✅ MySQL corriendo con `Database/*.sql` importados  
✅ `dotnet run` en Server → "Listening on port 7666"  
✅ `dotnet run` en Client → pantalla de login  
✅ Login username/password → lista de personajes  
✅ Seleccionar personaje → entra al mapa  
✅ Mover WASD → personaje se mueve, otros lo ven  
✅ ENTER + mensaje → chat sobre la cabeza  
✅ Cerrar cliente → posición guardada en DB  
✅ Reconectar → personaje aparece donde se dejó  

---

## Archivos Críticos a Consultar

- `Imperium AO 1/Server/Codigo/Protocol.bas` — PacketIDs exactos
- `Imperium AO 1/Fixtures/Base de datos/iaccuentas.sql` — schema cuentas
- `Imperium AO 1/Fixtures/Base de datos/iacuser.sql` — schema personajes
- `Imperium AO 2/Common/Network/ByteBuffer.cs` — reutilizar serialización

---

**Fecha**: 2026-05-03  
**Versión**: 1.0  
**Estado**: Plan aprobado, listo para implementar
