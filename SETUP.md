# Setup Completo - Imperium AO

Guía detallada para configurar el entorno de desarrollo y ejecutar por primera vez.

## Paso 1: Prerequisitos del Sistema

### Windows 10/11

1. **Instalar .NET 8 SDK**
   ```bash
   # Descargar desde https://dotnet.microsoft.com/download/dotnet/8.0
   # O usando winget
   winget install Microsoft.DotNet.SDK.8
   ```

2. **Verificar instalación**
   ```bash
   dotnet --version
   # Debe mostrar 8.x.x
   ```

3. **Instalar SQL Server LocalDB**
   ```bash
   # Opción A: Con Visual Studio (recomendado)
   # Durante la instalación de VS, selecciona "SQL Server Express LocalDB"
   
   # Opción B: Descarga directa
   # https://learn.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb
   ```

4. **Verificar LocalDB**
   ```bash
   SqlLocalDB.exe info
   # Debería mostrar mssqllocaldb en la lista
   
   # Si no existe, crearla
   SqlLocalDB.exe create mssqllocaldb
   SqlLocalDB.exe start mssqllocaldb
   ```

5. **Instalar Git** (opcional, pero recomendado)
   ```bash
   winget install Git.Git
   ```

## Paso 2: Clonar o Descargar el Repositorio

### Opción A: Con Git
```bash
cd C:\Development  # O donde prefieras
git clone https://github.com/Benicere/imperium-ao.git
cd imperium-ao
```

### Opción B: Descargar ZIP
1. Ve a https://github.com/Benicere/imperium-ao
2. Click "Code" → "Download ZIP"
3. Extrae en tu carpeta preferida

## Paso 3: Configurar el Proyecto

### Restaurar Dependencias
```bash
cd "Imperium AO 2"
dotnet restore
```

Esto descargará todos los paquetes NuGet necesarios (~200MB):
- Entity Framework Core
- MonoGame
- Serilog
- Microsoft.Extensions.*

### Compilar el Proyecto
```bash
dotnet build
```

Debería ver:
```
Build succeeded.
```

## Paso 4: Configurar la Base de Datos

### Verificar la Conexión

El servidor configurará la BD automáticamente, pero puedes verificar:

```bash
# Conectarse a LocalDB
sqlcmd -S (localdb)\mssqllocaldb

# En el prompt de SQL
1> CREATE DATABASE ImperiumAO_Test;
2> GO
3> SELECT name FROM sys.databases WHERE name = 'ImperiumAO_Test';
4> GO
5> DROP DATABASE ImperiumAO_Test;
6> GO
7> EXIT
```

### Estructura de la Base de Datos

El servidor creará automáticamente:

**Tabla: Accounts**
```sql
CREATE TABLE Accounts (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL UNIQUE,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(MAX) NOT NULL,
    IsActive BIT DEFAULT 1,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    LastLogin DATETIME2
);
```

**Tabla: Character**
```sql
CREATE TABLE Character (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Discriminator NVARCHAR(MAX),  -- Tipo (Player o Npc)
    Name NVARCHAR(50) NOT NULL,
    Level INT DEFAULT 1,
    Experience INT DEFAULT 0,
    Health INT,
    MaxHealth INT,
    Mana INT,
    MaxMana INT,
    Stamina INT,
    MaxStamina INT,
    X INT DEFAULT 0,
    Y INT DEFAULT 0,
    Map INT DEFAULT 0,
    LastUpdated DATETIME2,
    
    -- Campos específicos de Player
    AccountId INT,
    Strength INT,
    Intelligence INT,
    Constitution INT,
    Dexterity INT,
    Wisdom INT,
    Charisma INT,
    Gold INT,
    Bank INT,
    CreatedAt DATETIME2,
    Class INT,
    Race INT,
    Gender INT,
    
    -- Campos específicos de NPC
    NpcTypeId INT,
    
    FOREIGN KEY (AccountId) REFERENCES Accounts(Id) ON DELETE CASCADE
);

CREATE INDEX IX_Character_AccountId ON Character(AccountId);
```

### Verificar las Tablas (Después de ejecutar el servidor)

```bash
# Conectarse a la BD creada
sqlcmd -S (localdb)\mssqllocaldb -d ImperiumAO

# Ver tablas
1> SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES;
2> GO

# Ver estructura de Accounts
3> EXEC sp_help 'dbo.Accounts';
4> GO

# Ver datos (inicialmente vacío)
5> SELECT * FROM Accounts;
6> GO
```

## Paso 5: Ejecutar el Servidor

En una terminal PowerShell o CMD:

```bash
cd "Imperium AO 2\Server"
dotnet run
```

Deberías ver:

```
[09:15:30 INF] Starting ImperiumAO Server
[09:15:30 INF] Database migrations applied successfully
[09:15:30 INF] Starting TCP Network Server on port 7666
[09:15:30 INF] Server ready, waiting for connections
```

### Esperado
- ✅ No hay errores
- ✅ Base de datos creada (se puede verificar con Management Studio)
- ✅ Puerto 7666 en listen
- ✅ Logs aparecen en `/logs/imperiumao-YYYYMMDD.txt`

## Paso 6: Ejecutar el Cliente

En otra terminal:

```bash
cd "Imperium AO 2\Client"
dotnet run
```

Debería abrirse una ventana 1024x768 con:
- Título: Imperium AO
- Fondo: Azul acero
- Pantalla de login

## Paso 7: Prueba de Conexión

1. **En el cliente** escribe:
   - Usuario: `test`
   - Contraseña: `123`

2. Presiona **ENTER**

3. Espera ~2 segundos

4. Debería mostrar:
   - ✅ "¡Login exitoso!" (si todo funciona)
   - ❌ "Error: [mensaje]" (si hay un problema)

### Troubleshooting de Conexión

Si ves "Error: Cannot connect to server":

**Opción 1: Verificar puerto**
```bash
netstat -ano | findstr 7666
# Debería mostrar un proceso en LISTEN
```

**Opción 2: Firewall**
```bash
# Permitir .NET en firewall (Windows)
# O simplemente desactiva firewall temporalmente para testing
```

**Opción 3: Logs**
Revisa `/logs/` en la carpeta del servidor para ver qué salió mal

## Paso 8: Primera Prueba - Crear Personaje

Una vez logueado (si implementas login exitoso):

1. Presiona `W` para mover arriba
2. Presiona `S` para mover abajo
3. Presiona `A` para mover izquierda
4. Presiona `D` para mover derecha

El servidor recibirá los movimientos en consola.

## Paso 9: Configuración Avanzada

### Cambiar Base de Datos

En `appsettings.json`:

**Para SQL Server en red:**
```json
"ConnectionString": "Server=192.168.1.100;Database=ImperiumAO;User Id=sa;Password=YourPassword;"
```

**Para SQL Server en la nube (Azure):**
```json
"ConnectionString": "Server=myserver.database.windows.net;Database=ImperiumAO;User Id=admin@myserver;Password=YourPassword;Encrypt=true;Connection Timeout=30;"
```

### Cambiar Puerto del Servidor

En `appsettings.json`:
```json
"Server": {
  "Port": 8000,  // Cambiar a tu puerto
  "MaxConnections": 500
}
```

### Nivel de Logs

En `appsettings.json`:
```json
"Serilog": {
  "MinimumLevel": "Debug"  // Verbose, Debug, Information, Warning, Error, Fatal
}
```

## Paso 10: Desarrollo Continuo

### Reconstruir después de cambios
```bash
dotnet clean
dotnet build
```

### Ejecutar tests (cuando existan)
```bash
dotnet test
```

### Crear migraciones de BD
```bash
cd Server
dotnet ef migrations add MigrationName
```

## Checklist Final

- [ ] .NET 8 SDK instalado
- [ ] SQL Server LocalDB instalado y ejecutándose
- [ ] Proyecto clonado/descargado
- [ ] `dotnet restore` completado sin errores
- [ ] `dotnet build` exitoso (0 errores)
- [ ] Servidor inicia sin errores
- [ ] Cliente se abre sin crashes
- [ ] Cliente conecta a servidor (o intenta)

## ¿Qué sigue?

1. **Implementar Login Real**
   - Hash de contraseñas (bcrypt)
   - Verificación contra DB
   - Tokens de sesión

2. **Persistencia de Personajes**
   - Guardar en DB
   - Cargar al login
   - Sincronizar cambios

3. **Más Sistemas**
   - Combate real
   - Items y equipamiento
   - Magias
   - NPCs con IA

## Contacto y Soporte

Si tienes problemas:
1. Revisa los logs (`/logs/` o consola)
2. Verifica todos los requisitos están instalados
3. Intenta `dotnet clean && dotnet build`
4. Abre un issue en GitHub: https://github.com/Benicere/imperium-ao/issues

---

**¡A desarrollar!** 🚀
