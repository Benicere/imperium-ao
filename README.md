# Imperium AO - MMORPG Migration

Un MMORPG clásico migrado desde VB6 a C# .NET 8 con arquitectura moderna.

## Estado Actual

✅ **Compilación**: 0 errores, build exitoso  
✅ **Base de datos**: SQL Server LocalDB con EF Core  
✅ **Servidor**: Networking TCP/IP, 25 sistemas de juego  
✅ **Cliente**: MonoGame con interfaz mejorada  
✅ **Versionado**: Git + GitHub  

## Requisitos Previos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server LocalDB](https://learn.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb) (se instala automáticamente con Visual Studio)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) o [VS Code](https://code.visualstudio.com/)

## Instalación

1. **Clonar el repositorio**
```bash
git clone https://github.com/Benicere/imperium-ao.git
cd imperium-ao
```

2. **Restaurar dependencias**
```bash
cd "Imperium AO 2"
dotnet restore
```

3. **Compilar el proyecto**
```bash
dotnet build
```

## Ejecución

### Opción 1: Ejecutar Servidor + Cliente

**Terminal 1 - Servidor:**
```bash
cd "Imperium AO 2/Server"
dotnet run
```

El servidor:
- Creará automáticamente la base de datos ImperiumAO en LocalDB
- Aplicará las migraciones de EF Core
- Escuchará en puerto 7666
- Mostrará logs de inicio en consola

**Terminal 2 - Cliente:**
```bash
cd "Imperium AO 2/Client"
dotnet run
```

El cliente:
- Abrirá ventana de 1024x768
- Mostrará pantalla de login
- Se conectará a localhost:7666

### Credenciales de Prueba

El sistema está configurado para aceptar cualquier usuario/contraseña válidos. Para crear una cuenta:

1. Ejecuta el servidor
2. En el cliente, usa:
   - **Usuario**: `testuser`
   - **Contraseña**: `123456`

## Estructura del Proyecto

```
Imperium AO 2/
├── Common/
│   ├── Database/          # Repositories e interfaces
│   ├── Entities/          # Modelos de datos (Account, Player, NPC)
│   └── Network/           # Protocolo de red (PacketId, ByteBuffer)
│
├── Server/
│   ├── Program.cs         # Punto de entrada
│   ├── ServiceConfiguration.cs  # Inyección de dependencias
│   ├── Data/              # DbContext y migraciones EF Core
│   ├── Systems/           # 25 sistemas de juego (Combat, Skills, etc)
│   ├── Handlers/          # Manejadores de paquetes de red
│   ├── Core/              # GameServer, gestión central
│   ├── Network/           # Servidor TCP, dispatcher
│   ├── Map/               # Sistema de mapas
│   └── appsettings.json   # Configuración
│
├── Client/
│   ├── Program.cs         # Punto de entrada MonoGame
│   ├── ImperiumGame.cs    # Game loop principal
│   ├── Screens/           # Pantallas (Login, Game)
│   ├── Network/           # Cliente TCP, manejador de paquetes
│   ├── GameState/         # Estado local del juego
│   ├── UI/                # Sistema de UI
│   └── Content/           # Recursos (texturas, fuentes)
│
└── .git/                  # Historial de versiones
```

## Sistemas Implementados

### Core ✅
- Network TCP/IP con protocolo binario
- Dependency Injection (Microsoft.Extensions)
- Logging estructurado con Serilog
- Database con Entity Framework Core

### Combat ⚙️
- Resolver de combate
- Stats de atacante/defensor
- Efectos especiales

### Skills ⚙️
- Sistema de habilidades por tipo
- Leveling progresivo
- Bonificadores por nivel

### Inventory ⚙️
- Sistema de inventario
- Equipo de personaje
- Drop de items

### NPCs & AI ⚙️
- Gestor de NPCs
- Pathfinding A*
- Sistema de diálogo

### Social ⚙️
- Gremios
- Partidos
- Chat (global, whisper, gremio, party)
- PvP y facciones

### Otros ⚙️
- Trading
- Quests
- Magia avanzada
- Mascotas/Summons
- Forum
- Estadísticas
- Efectos visuales
- Audio
- UI avanzada

## Configuración

### appsettings.json

```json
{
  "Database": {
    "ConnectionString": "Server=(localdb)\\mssqllocaldb;Database=ImperiumAO;Integrated Security=true;"
  },
  "Server": {
    "Port": 7666,
    "MaxConnections": 500
  }
}
```

Para cambiar a un servidor SQL diferente:
```json
"ConnectionString": "Server=your-server;Database=ImperiumAO;User Id=sa;Password=your-password;"
```

## Desarrollo

### Agregar una nueva característica

1. Crear interfaz en `Common/`
2. Implementar en `Server/Systems/`
3. Registrar en `ServiceConfiguration.cs`
4. Crear handler en `Server/Handlers/` si requiere red

### Crear una migración

```bash
cd Server
dotnet ef migrations add MigrationName --context ImperiumAOContext --output-dir Data/Migrations
```

Las migraciones se aplican automáticamente al iniciar el servidor.

### Debugging

```bash
cd "Imperium AO 2"
dotnet build --configuration Debug
# Abrir en Visual Studio para debugging con breakpoints
```

## Troubleshooting

### "Cannot connect to server"
- Verifica que el servidor esté ejecutándose
- Comprueba puerto 7666: `netstat -an | findstr 7666`
- Revisa logs del servidor para errores

### "Database initialization failed"
- Verifica que LocalDB está instalado
- Revisa la cadena de conexión en appsettings.json
- Abre LocalDB: `SqlLocalDB.exe start mssqllocaldb`

### "Build failed"
- Limpia y reconstruye: `dotnet clean && dotnet build`
- Restaura paquetes: `dotnet restore`
- Verifica .NET 8: `dotnet --version`

## Performance

### Recomendaciones
- Server corre en threading nativo (System.IO.Pipelines)
- Client usa MonoGame (renderizado optimizado)
- Database con índices en tablas principales
- Async/await en toda la stack

### Monitoreo
```bash
# Logs en tiempo real
tail -f Imperium\ AO\ 2/Server/logs/imperiumao-*.txt

# Estadísticas de BD
SELECT name, row_count FROM sys.dm_db_partition_stats WHERE index_id <= 1
```

## Roadmap

- [ ] Sistema de login con contraseñas hasheadas
- [ ] Persistencia de personajes en DB
- [ ] Sincronización en tiempo real de movimientos
- [ ] Instancias y dungeons
- [ ] Economía de trading avanzada
- [ ] Guild wars
- [ ] Batalla real (BattleRoyale mode)

## Contribuciones

El proyecto está en desarrollo activo. Para contribuir:

1. Fork el repositorio
2. Crea una rama para tu feature: `git checkout -b feature/amazing-feature`
3. Commit tus cambios: `git commit -m 'Add amazing feature'`
4. Push a la rama: `git push origin feature/amazing-feature`
5. Abre un Pull Request

## Licencia

Este proyecto está bajo licencia MIT. Ver `LICENSE` para más detalles.

## Contacto

**Autor Original (VB6)**: Equipo Imperium AO  
**Migración a C#**: Augusto Mantero (@Benicere)

---

**Última actualización**: 2026-05-03  
**Versión**: 1.0.0 (Beta - Compilación exitosa)
