@echo off
echo Starting WasteNaut from project root...

REM Check if Ollama is installed and start it
echo Checking for Ollama...
ollama --version >nul 2>&1
if %errorlevel% equ 0 (
    echo Ollama found! Attempting to start Ollama service...
    start /B ollama serve >nul 2>&1
    timeout /t 2 /nobreak >nul
    echo Ollama service check completed
    echo Note: Make sure you have downloaded a model for AI features to work.
) else (
    echo Ollama not found. Project will run without AI features.
    echo To enable AI features, install Ollama from: https://ollama.ai/download
)

echo Starting WasteNaut project...
dotnet run --project backend/api/aspnet/WasteNaut.Admin
