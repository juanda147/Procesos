@echo off
echo ============================================
echo   Procesos - Sistema de Gestion Legal
echo ============================================
echo.
echo Iniciando servidor...
echo Acceda desde este PC: http://localhost:5050
echo.

REM Get local IP for LAN access
for /f "tokens=2 delims=:" %%a in ('ipconfig ^| findstr /c:"IPv4"') do (
    for /f "tokens=1" %%b in ("%%a") do (
        echo Acceso desde otra PC en la red: http://%%b:5050
    )
)
echo.
echo Presione Ctrl+C para detener el servidor.
echo ============================================
echo.

cd /d "%~dp0backend\ProcesosApi"
dotnet run
