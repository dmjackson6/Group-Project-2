@echo off
REM Enhanced WasteNaut startup script with Ollama integration
REM This script automatically starts Ollama if available and ensures the project runs

echo ========================================
echo    WasteNaut Startup Script
echo ========================================
echo.

REM Check if Ollama is installed and start it
echo [1/3] Checking for Ollama...
ollama --version >nul 2>&1
if %errorlevel% equ 0 (
    echo ✓ Ollama found! Version:
    ollama --version
    
    echo.
    echo [2/3] Checking Ollama service status...
    netstat -an | findstr "11434" >nul 2>&1
    if %errorlevel% equ 0 (
        echo ✓ Ollama service is already running on localhost:11434
    ) else (
        echo Starting Ollama service...
        start /B ollama serve
        timeout /t 3 /nobreak >nul
        echo ✓ Ollama service started on localhost:11434
    )
    echo ⚠ Note: Make sure you have downloaded a model (e.g., ollama pull llama3.2:3b) for AI features to work.
) else (
    echo ⚠ Ollama not found. Project will run without AI features.
    echo    To enable AI features:
    echo    1. Download Ollama from: https://ollama.ai/download
    echo    2. Install it on your system
    echo    3. Download a model: ollama pull llama3.2:3b
    echo    4. Restart this script
)

echo.
echo [3/3] Starting WasteNaut project...
echo ========================================
echo    Starting .NET Application
echo ========================================
echo.

REM Start the .NET project
dotnet run --project backend/api/aspnet/WasteNaut.Admin

REM Keep window open if there's an error
if %errorlevel% neq 0 (
    echo.
    echo ⚠ Project failed to start. Press any key to exit...
    pause >nul
)
