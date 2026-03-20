@echo off
echo ============================================
echo   Procesos - Sistema de Gestion Legal
echo ============================================
echo.
echo Verificando MongoDB...

REM Check if MongoDB is running
sc query MongoDB >nul 2>&1
if errorlevel 1 (
    echo ADVERTENCIA: El servicio MongoDB no parece estar corriendo.
    echo Asegurese de que MongoDB este instalado e iniciado.
    echo.
)

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

cd /d "%~dp0"
ProcesosApi.exe
