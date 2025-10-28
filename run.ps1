# PowerShell script to run the project from root directory
Write-Host "Starting WasteNaut from project root..." -ForegroundColor Green

# Check if Ollama is installed and start it
Write-Host "Checking for Ollama..." -ForegroundColor Yellow
try {
    $ollamaVersion = ollama --version 2>$null
    if ($LASTEXITCODE -eq 0) {
        Write-Host "Ollama found! Checking if service is running..." -ForegroundColor Yellow
        $portCheck = netstat -an | Select-String ":11434"
        if ($portCheck) {
            Write-Host "✓ Ollama service is already running on localhost:11434" -ForegroundColor Green
        } else {
            Write-Host "Starting Ollama service..." -ForegroundColor Yellow
            Start-Process -FilePath "ollama" -ArgumentList "serve" -WindowStyle Hidden
            Start-Sleep -Seconds 3
            Write-Host "✓ Ollama service started on localhost:11434" -ForegroundColor Green
        }
        Write-Host "Note: Make sure you have downloaded a model (e.g., ollama pull llama3.2:3b) for AI features to work." -ForegroundColor Yellow
    }
} catch {
    Write-Host "Ollama not found. Project will run without AI features." -ForegroundColor Yellow
    Write-Host "To enable AI features, install Ollama from: https://ollama.ai/download" -ForegroundColor Cyan
}

Write-Host "Starting WasteNaut project..." -ForegroundColor Green
dotnet run --project backend/api/aspnet/WasteNaut.Admin

