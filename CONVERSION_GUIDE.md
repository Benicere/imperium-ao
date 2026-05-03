# Guía de Conversión ImperiumAO VB6 a C#

## Estructura del Proyecto

### Imperium AO 1 (VB6 - Original)
- `Cliente/` - Cliente Visual Basic 6
- `Server/` - Servidor Visual Basic 6
- `Fixtures/` - Archivos SQL de base de datos

### Imperium AO 2 (C# - Convertido)
- `Common/` - Clases compartidas entre cliente y servidor
- `Server/` - Servidor C# (.NET 8)
- `Client/` - Cliente C# (Windows Forms/.NET 8)

## Guía de Mapping VB6 a C#

### Módulos de Clase (.cls)

```vb6
' VB6
Public Class clsExample
    Private m_value As Integer
    
    Public Property Value() As Integer
        Get
            Value = m_value
        End Get
        Set(ByVal v As Integer)
            m_value = v
        End Set
    End Property
End Class
```

```csharp
// C#
public class Example
{
    private int _value;
    
    public int Value
    {
        get => _value;
        set => _value = value;
    }
}
```

### Módulos Base (.bas)

Los módulos de base (funciones globales) deben convertirse a clases estáticas:

```vb6
' VB6
Public Function Add(a As Integer, b As Integer) As Integer
    Add = a + b
End Function
```

```csharp
// C#
public static class MathUtils
{
    public static int Add(int a, int b) => a + b;
}
```

### Base de Datos

**Cambio importante**: La conexión ODBC de VB6 se reemplaza con MySqlConnector:

```csharp
using MySql.Data.MySqlClient;

public class CharacterRepository
{
    private readonly string _connectionString;
    
    public async Task<Player> LoadPlayerAsync(int playerId)
    {
        using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();
        
        var command = new MySqlCommand("SELECT * FROM characters WHERE id = @id", connection);
        command.Parameters.AddWithValue("@id", playerId);
        
        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new Player
            {
                Id = reader.GetInt32("id"),
                Name = reader.GetString("name"),
                Level = reader.GetInt32("level"),
                // ... mapeo de otras columnas
            };
        }
        return null;
    }
}
```

### Conversión de Tipos

| VB6 | C# |
|-----|-----|
| `Integer` | `int` |
| `Long` | `long` |
| `Single` | `float` |
| `Double` | `double` |
| `String` | `string` |
| `Boolean` | `bool` |
| `Byte` | `byte` |
| `Date` | `DateTime` |
| `Object` | `object` |
| `Array()` | `List<T>` o `T[]` |
| `Collection` | `Dictionary<K,V>` o `List<T>` |

### Enumeraciones

```vb6
' VB6
Public Enum eClases
    Guerrero = 1
    Mago = 2
    Arquero = 3
End Enum
```

```csharp
// C#
public enum PlayerClass
{
    Guerrero = 1,
    Mago = 2,
    Arquero = 3
}
```

### Manejo de Errores

```vb6
' VB6
On Error GoTo ErrorHandler
' ... código
Exit Sub
ErrorHandler:
    MsgBox Err.Description
End Sub
```

```csharp
// C#
try
{
    // ... código
}
catch (Exception ex)
{
    _logger.LogError(ex, "Error message");
}
```

### Archivos de Configuración

**VB6** usaba archivos .INI, **C#** usa JSON:

```json
{
  "Database": {
    "CharacterConnectionString": "Server=localhost;User=root;Database=imperium_characters",
    "AccountConnectionString": "Server=localhost;User=root;Database=imperium_accounts"
  },
  "Server": {
    "Port": 7666,
    "MaxPlayers": 1000
  }
}
```

## Módulos VB6 a Convertir

### Server (71 módulos)

**Módulos Críticos:**
- `Acciones.bas` → `Systems/ActionSystem.cs`
- `Characters.bas` → `Entities/Character.cs` + `Systems/CharacterSystem.cs`
- `Admin.bas` → `Systems/AdminSystem.cs`
- `AI_NPC.bas` → `Systems/AiNpcSystem.cs`
- `clsColaArray.cls` → `Collections/Queue.cs`
- `clsByteBuffer.cls` → `Collections/ByteBuffer.cs`
- `Combat.bas` → `Systems/CombatSystem.cs`
- `Skills.bas` → `Systems/SkillSystem.cs`
- `Clans.bas` → `Systems/ClanSystem.cs`

**Base de Datos:**
- `clsConexionDb.cls` → `Database/DatabaseConnection.cs`
- `SQL*.bas` → `Database/Repositories/`

### Cliente (VB6)

**Módulos Principales:**
- `Aplicacion/Application.bas` → `Application.cs`
- `Graficos/Graphics.cs`
- `Sonido/AudioManager.cs`
- `Interface/UIManager.cs`

## Proceso de Conversión

### Paso 1: Análisis
- [ ] Revisar cada módulo VB6
- [ ] Documentar dependencias
- [ ] Identificar patrones de diseño

### Paso 2: Diseño de Arquitectura C#
- [ ] Crear estructura de carpetas
- [ ] Definir interfaces
- [ ] Planificar inyección de dependencias

### Paso 3: Conversión
- [ ] Convertir entidades (Character, Player, NPC, Item)
- [ ] Convertir sistemas (Combat, Skill, AI)
- [ ] Convertir gestores (Player, World, Database)

### Paso 4: Testing
- [ ] Unit tests para lógica de negocio
- [ ] Integration tests para base de datos
- [ ] Pruebas de funcionalidad

## Mejoras en C#

1. **Null Safety**: Usar `#nullable enable` y tipos nullable
2. **Async/Await**: Convertir operaciones bloqueantes a async
3. **Logging**: Usar Serilog en lugar de MsgBox/Debug.Print
4. **Inyección de Dependencias**: DI integrada en .NET
5. **Entity Framework Core**: Opcional, para ORM

## Estado Actual

✅ Estructura base creada  
✅ Archivos de proyecto (.csproj)  
✅ Interfaces principales definidas  
✅ Clases base de entidades  
✅ Configuración JSON  

⏳ Conversión de módulos del server  
⏳ Conversión de cliente  
⏳ Sistema de base de datos  
⏳ Testing  

## Ejecutar el Servidor

```bash
cd "d:\Proyectos_Codigo\Imperium AO Raiz\Imperium AO 2\Server"
dotnet run --configuration Release
```

## Enlaces Útiles

- [VB6 to C# Conversion Guide](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/)
- [MySqlConnector](https://mysqlconnector.net/)
- [Serilog Logging](https://serilog.net/)
- [Dependency Injection in .NET](https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection)
