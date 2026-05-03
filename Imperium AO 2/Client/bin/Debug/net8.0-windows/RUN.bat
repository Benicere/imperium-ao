@echo off
cd /d "%~dp0"
echo ====================================
echo IMPERIUM AO - CLIENT
echo ====================================
echo.
echo Starting client from: %cd%
echo.

ImperiumAO.Client.exe

if %ERRORLEVEL% neq 0 (
    echo.
    echo ERROR: El cliente se cerro con codigo: %ERRORLEVEL%
    echo.
    pause
)
