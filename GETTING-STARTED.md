# 🚀 Comenzar en 5 Minutos

Guía ultrarrápida para ejecutar Imperium AO.

## Paso 1: Verificar Requisitos

```powershell
# Abrir PowerShell y ejecutar:
dotnet --version
# ¿Muestra 8.x.x? ✅ Bien, continúa
# ¿Muestra error? ❌ Instala .NET 8 desde https://dotnet.microsoft.com/download/dotnet/8.0
```

## Paso 2: Descargar Código

### Opción A: Con Git
```bash
git clone https://github.com/Benicere/imperium-ao.git
cd imperium-ao
```

### Opción B: ZIP
1. Descarga: https://github.com/Benicere/imperium-ao/archive/refs/heads/main.zip
2. Extrae en tu carpeta preferida
3. Abre PowerShell en esa carpeta

## Paso 3: Compilar

```powershell
.\build.ps1
```

**Espera a ver**:
```
✅ Build exitoso!
```

Si ves error: Revisa SETUP.md

## Paso 4: Ejecutar

Abre **3 PowerShells** en la misma carpeta:

### PowerShell #1 - Servidor
```powershell
.\run-server.ps1
```

Espera a ver:
```
[INFO] Starting ImperiumAO Server
[INFO] Database migrations applied successfully
[INFO] Server ready, waiting for connections
```

### PowerShell #2 - Cliente
```powershell
.\run-client.ps1
```

Debería abrirse una ventana de juego.

### PowerShell #3 - (Opcional) Logs
```powershell
Get-Content -Path ".\Imperium AO 2\Server\logs\imperiumao-*.txt" -Wait
```

## Paso 5: Probar

En el cliente (ventana de juego):

1. Escribe usuario: `test`
2. Escribe contraseña: `123`
3. Presiona **ENTER**

Debería decir "Conectando..." y luego algo (login/error).

✅ **¡Listo!** Ya funciona.

## ¿Qué Sigue?

- Lee [README.md](README.md) para entender la estructura
- Lee [PROGRESS.md](PROGRESS.md) para ver qué se puede mejorar
- Lee [SETUP.md](SETUP.md) si tienes problemas

## 🆘 Si Algo Falla

### Error: "build.ps1 not found"
```powershell
# Asegúrate de estar en la carpeta raíz
cd C:\ruta\a\imperium-ao
ls build.ps1
```

### Error: "PowerShell no permite scripts"
```powershell
Set-ExecutionPolicy -ExecutionPolicy Bypass -Scope CurrentUser -Force
```

### Error: "El servidor no inicia"
- ¿Está SQL Server LocalDB instalado?
- Intenta: `SqlLocalDB.exe start mssqllocaldb`

### Error: "El cliente no conecta"
- Verifica que el servidor esté en PowerShell #1
- Espera 2 segundos más
- Revisa consola del servidor para errores

### Error: "dotnet not found"
- Instala .NET 8: https://dotnet.microsoft.com/download/dotnet/8.0
- Cierra y reabre PowerShell

## 🎮 Próximos Pasos

Una vez que funciona:

1. **Explorar Código**
   ```powershell
   # Abrir en Visual Studio 2022
   .\Imperium AO 2\ImperiumAO.CSharp.sln
   ```

2. **Hacer Cambios**
   - Edita archivos .cs
   - Ejecuta `.\build.ps1` nuevamente
   - Reinicia servidor/cliente

3. **Guardar Cambios**
   ```bash
   git add .
   git commit -m "Cambio: descripción"
   git push
   ```

## 📊 Estado Actual

```
Compilación:    ✅ 0 errores
Servidor:       ✅ Escucha en 7666
Cliente:        ✅ Renderiza
Base de datos:  ✅ Auto-crea
Git:            ✅ Sincronizado
```

## 🎯 Objetivo Final

El juego completo funcionando con:
- ✅ Login real
- ✅ Personajes guardados
- ✅ NPCs y mobs
- ✅ Combate
- ✅ Items y quests
- ✅ Multijugador real

## 💬 Preguntas?

1. **¿Cómo cambio el puerto del servidor?**
   - Edita `Imperium AO 2\Server\appsettings.json`, línea "Port"

2. **¿Cómo cambio la BD a SQL Server en red?**
   - Edita `Imperium AO 2\Server\appsettings.json`, línea "ConnectionString"

3. **¿Dónde están los logs?**
   - `Imperium AO 2\Server\logs\imperiumao-*.txt`

4. **¿Cómo debugueo?**
   - Abre en Visual Studio 2022 con F5

5. **¿Puedo hacer push a GitHub?**
   - Sí: `git push` (requiere acceso al repo)

---

**¡A jugar!** 🎮

Si necesitas ayuda: Abre issue en GitHub  
Si encontraste bug: Reporta con pasos para reproducir  
Si mejoraste algo: Haz PR
