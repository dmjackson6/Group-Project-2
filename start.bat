@echo off
echo Starting WasteNaut...

REM Check if Ollama is installed and start it
echo Checking for Ollama...
ollama --version >nul 2>&1
if %errorlevel% equ 0 (
    echo Ollama found! Checking if service is running...
    netstat -an | findstr "11434" >nul 2>&1
    if %errorlevel% equ 0 (
        echo ✓ Ollama service is already running on localhost:11434
    ) else (
        echo Starting Ollama service...
        start /B ollama serve
        timeout /t 3 /nobreak >nul
        echo ✓ Ollama service started on localhost:11434
    )
    echo Note: Make sure you have downloaded a model (e.g., ollama pull llama3.2:3b) for AI features to work.
) else (
    echo Ollama not found. Project will run without AI features.
    echo To enable AI features, install Ollama from: https://ollama.ai/download
)

echo Starting WasteNaut project...
dotnet run --project backend/api/aspnet/WasteNaut.Admin

