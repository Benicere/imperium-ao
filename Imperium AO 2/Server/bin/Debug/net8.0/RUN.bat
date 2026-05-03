@echo off
cd /d "%~dp0"
echo ====================================
echo IMPERIUM AO - SERVER
echo ====================================
echo.
echo Starting server on port 7666...
echo Database: SQL Server LocalDB
echo.

ImperiumAO.Server.exe

if %ERRORLEVEL% neq 0 (
    echo.
    echo ERROR: El servidor se cerro con codigo: %ERRORLEVEL%
    echo.
    pause
)
