@echo off
echo ========================================
echo 🚀 WasteNaut Complete System Startup
echo ========================================
echo.

echo 📋 Checking prerequisites...
echo.

REM Check if .NET is installed
dotnet --version >nul 2>&1
if %errorlevel% neq 0 (
    echo ❌ .NET is not installed or not in PATH
    echo Please install .NET 9.0 or later
    pause
    exit /b 1
)
echo ✅ .NET is installed

REM Check if MySQL is running
net start | findstr /i "mysql" >nul 2>&1
if %errorlevel% neq 0 (
    echo ⚠️  MySQL service not found or not running
    echo Please ensure MySQL is installed and running
    echo.
)

echo.
echo 🔧 Setting up the system...
echo.

REM Navigate to project directory
cd /d "%~dp0"

REM Restore NuGet packages
echo 📦 Restoring NuGet packages...
cd backend\api\aspnet\WasteNaut.Admin
dotnet restore
if %errorlevel% neq 0 (
    echo ❌ Failed to restore packages
    pause
    exit /b 1
)
echo ✅ Packages restored

REM Build the project
echo 🔨 Building the project...
dotnet build
if %errorlevel% neq 0 (
    echo ❌ Build failed
    pause
    exit /b 1
)
echo ✅ Project built successfully

echo.
echo 🤖 Checking for Ollama...
ollama --version >nul 2>&1
if %errorlevel% neq 0 (
    echo ⚠️  Ollama not found - AI features will be limited
    echo You can install Ollama from https://ollama.ai
    echo.
) else (
    echo ✅ Ollama found
    echo 🚀 Starting Ollama service...
    start /b ollama serve
    timeout /t 3 >nul
    echo ✅ Ollama service started
)

echo.
echo 🌐 Starting WasteNaut server...
echo.
echo 📍 Server will be available at: http://localhost:3000
echo 📍 API documentation at: http://localhost:3000/swagger
echo 📍 Test suite at: http://localhost:3000/test-complete-system.html
echo.
echo Press Ctrl+C to stop the server
echo.

REM Start the application
dotnet run --urls "http://localhost:3000"

pause
