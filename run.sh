#!/bin/bash
# Cross-platform script to run WasteNaut with Ollama integration

echo "Starting WasteNaut..."

# Check if Ollama is installed and start it
echo "Checking for Ollama..."
if command -v ollama &> /dev/null; then
    echo "Ollama found! Checking if service is running..."
    if netstat -an 2>/dev/null | grep -q ":11434" || ss -tuln 2>/dev/null | grep -q ":11434"; then
        echo "✓ Ollama service is already running on localhost:11434"
    else
        echo "Starting Ollama service..."
        ollama serve &
        sleep 3
        echo "✓ Ollama service started on localhost:11434"
    fi
    echo "Note: Make sure you have downloaded a model (e.g., ollama pull llama3.2:3b) for AI features to work."
else
    echo "Ollama not found. Project will run without AI features."
    echo "To enable AI features, install Ollama from: https://ollama.ai/download"
fi

echo "Starting WasteNaut project..."
dotnet run --project backend/api/aspnet/WasteNaut.Admin
