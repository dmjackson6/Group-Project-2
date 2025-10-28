#!/bin/bash
# Ollama Setup Script for WasteNaut
# This script helps you set up Ollama for your smart matching page

echo "🤖 Setting up Ollama for WasteNaut Smart Matching"
echo "=================================================="

# Check if Ollama is installed
if ! command -v ollama &> /dev/null; then
    echo "❌ Ollama is not installed."
    echo "📥 Installing Ollama..."
    
    # Install Ollama based on OS
    if [[ "$OSTYPE" == "linux-gnu"* ]]; then
        curl -fsSL https://ollama.ai/install.sh | sh
    elif [[ "$OSTYPE" == "darwin"* ]]; then
        echo "Please download Ollama from https://ollama.ai/download for macOS"
        exit 1
    elif [[ "$OSTYPE" == "msys" ]] || [[ "$OSTYPE" == "win32" ]]; then
        echo "Please download Ollama from https://ollama.ai/download for Windows"
        exit 1
    fi
else
    echo "✅ Ollama is already installed"
fi

# Start Ollama service
echo "🚀 Starting Ollama service..."
ollama serve &
OLLAMA_PID=$!

# Wait for Ollama to start
echo "⏳ Waiting for Ollama to start..."
sleep 5

# Pull recommended model
echo "📦 Downloading recommended model (llama3.2:3b)..."
ollama pull llama3.2:3b

# Test the model
echo "🧪 Testing the model..."
TEST_RESPONSE=$(ollama run llama3.2:3b "Hello, I need help finding food donations" --format json 2>/dev/null)

if [ $? -eq 0 ]; then
    echo "✅ Ollama is working correctly!"
    echo ""
    echo "🎉 Setup Complete!"
    echo "=================="
    echo "Your WasteNaut smart matching page is now ready to use Ollama AI!"
    echo ""
    echo "📋 Next Steps:"
    echo "1. Open your smart-matching.html page"
    echo "2. Ask the AI assistant: 'I need vegetarian food for a family of 4'"
    echo "3. Enjoy your free, local AI-powered food matching!"
    echo ""
    echo "🔧 Useful Commands:"
    echo "- Check running models: ollama ps"
    echo "- List installed models: ollama list"
    echo "- Stop Ollama: pkill ollama"
    echo "- Restart Ollama: ollama serve"
else
    echo "❌ Ollama test failed. Please check the installation."
    echo "💡 Try running: ollama serve"
    echo "💡 Then test with: ollama run llama3.2:3b 'Hello'"
fi

echo ""
echo "📚 For more help, see: OLLAMA_SETUP_GUIDE.md"
