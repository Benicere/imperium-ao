@echo off
title IMPERIUM AO - LAUNCHER
color 0A

cls
echo.
echo ========================================
echo     IMPERIUM AO - MMORPG LAUNCHER
echo ========================================
echo.
echo Iniciando componentes...
echo.

REM Iniciar servidor en una ventana separada
echo [1/2] Iniciando SERVIDOR...
start "Imperium AO Server" "Imperium AO 2\Server\bin\Debug\net8.0\RUN.bat"

REM Esperar un poco para que el servidor inicie
timeout /t 3 /nobreak

REM Iniciar cliente
echo [2/2] Iniciando CLIENTE...
start "Imperium AO Client" "Imperium AO 2\Client\bin\Debug\net8.0-windows\RUN.bat"

echo.
echo ========================================
echo Ambos componentes iniciados.
echo - Servidor: ventana "Imperium AO Server"
echo - Cliente: ventana "Imperium AO Client"
echo ========================================
echo.
pause

exit
