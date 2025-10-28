# Ollama Integration Guide for WasteNaut

## 🤖 Complete Ollama Setup for Your Smart Matching Page

Ollama is the perfect choice for your WasteNaut platform - it's 100% free, runs locally, and gives you complete control over your AI.

## 🚀 Quick Setup (5 Minutes)

### Step 1: Install Ollama

**Windows:**
```bash
# Download from https://ollama.ai/download
# Or use PowerShell:
winget install Ollama.Ollama
```

**Mac:**
```bash
# Download from https://ollama.ai/download
# Or use Homebrew:
brew install ollama
```

**Linux:**
```bash
curl -fsSL https://ollama.ai/install.sh | sh
```

### Step 2: Pull a Model

```bash
# Lightweight model (recommended for your use case)
ollama pull llama3.2:3b

# Or more capable model
ollama pull mistral:7b

# Or latest Llama model
ollama pull llama3.1:8b
```

### Step 3: Start Ollama

```bash
# Start Ollama server
ollama serve
```

### Step 4: Test Your Setup

```bash
# Test with a simple query
ollama run llama3.2:3b "Hello, I need help finding food donations"
```

## 🔧 Your Smart Matching Page is Already Configured!

I've already updated your `smart-matching.html` to work with Ollama. Here's what's included:

### ✅ **Features Already Implemented:**

1. **Ollama API Integration**
   - Connects to `http://localhost:11434/api/generate`
   - Uses your selected model (llama3.2:3b by default)
   - Handles errors gracefully with fallbacks

2. **Service Selector**
   - Choose between Ollama, Hugging Face, or Demo Mode
   - Visual indicators for each service
   - Easy switching between AI providers

3. **Smart Prompts**
   - Contextual prompts that include user preferences
   - Food donation specific instructions
   - Personalized responses based on available data

4. **Error Handling**
   - Automatic fallback to demo mode if Ollama isn't running
   - Clear error messages
   - Graceful degradation

## 🎯 How It Works

### **User Asks Question:**
```
"I need vegetarian food for a family of 4"
```

### **Ollama Receives Context:**
```
You are an AI assistant for WasteNaut, a food donation matching platform.

User Query: "I need vegetarian food for a family of 4"

User Profile:
- Preferences: vegetarian
- Needs: fresh-produce
- Urgency: medium
- Max Distance: 10 miles

Available Donations:
[Fresh Organic Vegetables, Canned Protein Collection, etc.]

Please provide a helpful, personalized response...
```

### **Ollama Responds:**
```
Based on your vegetarian preferences and family size, I found excellent matches! 

🥇 **Fresh Organic Vegetables** (0.8 miles) - Perfect match! 50 lbs of mixed organic vegetables from Green Valley Farm. This matches your vegetarian preferences and is very close to you.

🥈 **Family Food Assistance** (0.5 miles) - They specifically help families of 4 and need fresh vegetables and dairy, which aligns perfectly with available donations.

Both options are within your 10-mile radius and match your dietary needs. The Fresh Organic Vegetables would be my top recommendation since it's organic and specifically vegetarian-friendly.
```

## 📊 Model Recommendations

### **For Your Use Case:**

| Model | Size | Speed | Quality | Best For |
|-------|------|-------|---------|----------|
| **llama3.2:3b** | 2GB | ⚡⚡⚡ | ⭐⭐⭐ | **Recommended** - Fast, good quality |
| **mistral:7b** | 4GB | ⚡⚡ | ⭐⭐⭐⭐ | Better quality, slower |
| **llama3.1:8b** | 5GB | ⚡ | ⭐⭐⭐⭐⭐ | Best quality, slowest |

### **Hardware Requirements:**

| Model | RAM | Storage | CPU |
|-------|-----|---------|-----|
| llama3.2:3b | 4GB | 2GB | Any |
| mistral:7b | 8GB | 4GB | Modern |
| llama3.1:8b | 10GB | 5GB | Modern |

## 🔧 Advanced Configuration

### **Customize Your Model:**

```bash
# Create a custom model for food donations
ollama create food-assistant -f Modelfile
```

**Modelfile:**
```
FROM llama3.2:3b

SYSTEM """You are a specialized AI assistant for WasteNaut, a food donation matching platform. You help people find food resources, understand dietary needs, and connect communities. Always be encouraging, helpful, and focused on food security."""
```

### **Performance Tuning:**

```javascript
// In your smart-matching.html, you can adjust these settings:
options: {
  temperature: 0.7,    // Creativity (0.1 = focused, 1.0 = creative)
  top_p: 0.9,         // Diversity (0.1 = focused, 1.0 = diverse)
  max_tokens: 300,    // Response length
  num_ctx: 2048      // Context window
}
```

## 🚀 Production Deployment

### **For Production Use:**

1. **Use a More Powerful Model:**
   ```bash
   ollama pull llama3.1:8b
   ```

2. **Optimize for Speed:**
   ```bash
   # Run with GPU acceleration
   OLLAMA_GPU=1 ollama serve
   ```

3. **Scale with Multiple Instances:**
   ```bash
   # Run multiple Ollama instances
   OLLAMA_HOST=0.0.0.0:11434 ollama serve
   OLLAMA_HOST=0.0.0.0:11435 ollama serve
   ```

## 🔒 Security & Privacy

### **Why Ollama is Perfect:**

✅ **100% Private** - No data leaves your server  
✅ **No API Keys** - No external dependencies  
✅ **No Usage Limits** - Unlimited requests  
✅ **No Costs** - Completely free  
✅ **Offline Capable** - Works without internet  

## 📈 Monitoring & Analytics

### **Track Ollama Performance:**

```javascript
function trackOllamaUsage(query, response, responseTime) {
  const metrics = {
    timestamp: new Date(),
    queryLength: query.length,
    responseLength: response.length,
    responseTime: responseTime,
    model: currentAIService.model
  };
  
  console.log('Ollama Metrics:', metrics);
  // Send to your analytics service
}
```

## 🛠️ Troubleshooting

### **Common Issues:**

1. **"Ollama API error: 404"**
   - Make sure Ollama is running: `ollama serve`
   - Check if model is installed: `ollama list`

2. **"Connection refused"**
   - Verify Ollama is running on port 11434
   - Check firewall settings

3. **Slow responses**
   - Try a smaller model: `llama3.2:3b`
   - Reduce max_tokens in options
   - Use GPU acceleration if available

### **Performance Tips:**

```bash
# Monitor Ollama performance
ollama ps

# Check model status
ollama list

# Restart Ollama if needed
pkill ollama && ollama serve
```

## 🎉 You're Ready!

Your WasteNaut smart matching page now has:

- ✅ **100% Free AI** - No API costs ever
- ✅ **Complete Privacy** - All data stays local
- ✅ **Unlimited Usage** - No rate limits
- ✅ **Easy Setup** - Works out of the box
- ✅ **Professional Quality** - Real AI responses

### **Next Steps:**

1. **Install Ollama** (5 minutes)
2. **Pull a model** (2 minutes)
3. **Start Ollama** (1 minute)
4. **Test your page** - Ask the AI assistant!

Your smart matching page is now powered by real AI, completely free, and ready for production use! 🚀
