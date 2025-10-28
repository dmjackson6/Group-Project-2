# WasteNaut Startup Scripts

This project now includes enhanced startup scripts that automatically handle Ollama integration.

## 🚀 Quick Start

### Windows Users
```bash
# Use any of these commands:
start-enhanced.bat    # Enhanced version with detailed output
run.bat              # Standard version
start.bat            # Simple version
```

### PowerShell Users
```powershell
.\run.ps1
```

### Linux/Mac Users
```bash
./run.sh
```

## 🤖 What the Scripts Do

### Automatic Ollama Integration
1. **Check for Ollama** - Detects if Ollama is installed
2. **Start Ollama Service** - Automatically starts `ollama serve`
3. **Start Project** - Runs `dotnet run --project backend/api/aspnet/WasteNaut.Admin`

### Graceful Fallback
- If Ollama is **not installed**: Project runs normally without AI features
- If Ollama **fails to start**: Project continues with fallback AI responses
- **No automatic model downloads** - Users must manually download models

## 📋 Manual Commands

If you prefer to run commands manually:

```bash
# 1. Start Ollama (if installed)
ollama serve

# 2. Download model (if needed) - USER MUST DO THIS MANUALLY
ollama pull llama3.2:3b

# 3. Start the project
dotnet run --project backend/api/aspnet/WasteNaut.Admin
```

## 🔧 Troubleshooting

### Ollama Not Starting
- **Windows**: Make sure Ollama is installed and in your PATH
- **Linux/Mac**: Ensure Ollama has proper permissions
- **All**: Check if port 11434 is available

### Model Download Issues
- **Manual Download Required**: Users must run `ollama pull <model>` themselves
- **Terms of Service**: We don't automatically download models to comply with ToS
- **Check Available Models**: Run `ollama list` to see installed models

### Project Won't Start
- Ensure .NET 8.0+ is installed
- Check if the project path is correct
- Verify all dependencies are restored

## 🎯 Features

### With Ollama Installed & Running
- ✅ Full AI-powered smart matching
- ✅ Real-time AI assistant
- ✅ Personalized recommendations
- ✅ Natural language processing

### Without Ollama or Models
- ✅ Project runs normally
- ✅ Fallback AI responses
- ✅ All other features work
- ✅ Setup guide available in UI

## 📱 Cross-Platform Support

| Platform | Script | Notes |
|----------|--------|-------|
| Windows | `start-enhanced.bat` | Recommended |
| Windows | `run.bat` | Standard |
| PowerShell | `run.ps1` | PowerShell users |
| Linux/Mac | `run.sh` | Unix systems |

## 🔗 Links

- **Ollama Download**: https://ollama.ai/download
- **Ollama Models**: https://ollama.ai/library
- **Ollama Docs**: https://ollama.ai/docs

## ⚠️ Important Notes

- **No Automatic Model Downloads**: We respect terms of service and don't download models automatically
- **Manual Model Installation**: Users must run `ollama pull <model>` themselves
- **Graceful Degradation**: Project works with or without Ollama/models
- **User Choice**: Users can navigate the program without AI features

---

**Note**: The startup scripts will start Ollama if installed, but users must manually download models for AI features to work.
