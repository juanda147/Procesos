@echo off
echo ============================================
echo   Publicando Procesos para distribucion
echo ============================================
echo.

REM Build frontend
echo [1/3] Compilando frontend...
cd /d "%~dp0frontend"
call npm run build
if errorlevel 1 (
    echo ERROR: Fallo la compilacion del frontend
    pause
    exit /b 1
)

REM Copy frontend to wwwroot
echo [2/3] Copiando frontend a wwwroot...
if exist "%~dp0backend\ProcesosApi\wwwroot" rmdir /s /q "%~dp0backend\ProcesosApi\wwwroot"
xcopy /s /e /i /q "%~dp0frontend\dist" "%~dp0backend\ProcesosApi\wwwroot"

REM Publish backend
echo [3/3] Publicando backend...
cd /d "%~dp0backend\ProcesosApi"
dotnet publish -c Release -o "%~dp0publicacion"
if errorlevel 1 (
    echo ERROR: Fallo la publicacion del backend
    pause
    exit /b 1
)

REM Copy startup script
copy /y "%~dp0iniciar-portable.bat" "%~dp0publicacion\iniciar.bat"

echo.
echo ============================================
echo   Publicacion completada!
echo   Carpeta: %~dp0publicacion
echo ============================================
echo.
echo Copie la carpeta "publicacion" al otro PC.
echo Requisitos en el otro PC:
echo   1. MongoDB Community Server
echo   2. .NET 9 ASP.NET Core Runtime
echo.
pause
