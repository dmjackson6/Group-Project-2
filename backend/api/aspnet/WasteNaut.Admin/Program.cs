using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using WasteNaut.Admin.Data;
using WasteNaut.Admin.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Add Entity Framework - Use MySQL database
// Temporarily disabled due to database connection issues
// builder.Services.AddDbContext<WasteNautDbContext>(options =>
//     options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"), 
//     Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.0-mysql")));

// Add JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "WasteNaut",
            ValidAudience = builder.Configuration["Jwt:Audience"] ?? "WasteNaut-Users",
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? "default-secret-key-for-development"))
        };
    });

// Add Authorization
builder.Services.AddAuthorization();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Add Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "WasteNaut Admin API", 
        Version = "v1",
        Description = "API for WasteNaut admin operations"
    });
    
    // Add JWT authentication to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// Add Repository Pattern
// Temporarily disabled due to database connection issues
// builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// Add Services
// Temporarily disabled due to database connection issues
// builder.Services.AddScoped<IAdminService, AdminService>();
// builder.Services.AddScoped<IUserService, UserService>();
// builder.Services.AddScoped<IOrganizationService, OrganizationService>();
// builder.Services.AddScoped<IDonationService, DonationService>();
// builder.Services.AddScoped<IReportService, ReportService>();
// builder.Services.AddScoped<IMatchService, MatchService>();
// builder.Services.AddScoped<IAuditService, AuditService>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "WasteNaut Admin API v1");
        c.RoutePrefix = "swagger"; // Move Swagger to /swagger
    });
}

// Serve static files from wwwroot
app.UseDefaultFiles();
app.UseStaticFiles();

// Only redirect to HTTPS in production
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

// Route root to frontend index.html
app.MapGet("/", () => Results.Redirect("/index.html"));


// Temporarily disabled due to database connection issues
// app.MapControllers();

// Auto-migrate database in development
// Temporarily disabled due to database connection issues
// if (app.Environment.IsDevelopment())
// {
//     using (var scope = app.Services.CreateScope())
//     {
//         var context = scope.ServiceProvider.GetRequiredService<WasteNautDbContext>();
//         context.Database.EnsureCreated();
//     }
// }

// Start Ollama service if available
try
{
    Console.WriteLine("🤖 Checking for Ollama...");
    
    // Check if Ollama is installed
    var ollamaCheck = System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
    {
        FileName = "ollama",
        Arguments = "--version",
        UseShellExecute = false,
        RedirectStandardOutput = true,
        RedirectStandardError = true,
        CreateNoWindow = true
    });
    
    if (ollamaCheck != null)
    {
        ollamaCheck.WaitForExit();
        if (ollamaCheck.ExitCode == 0)
        {
            Console.WriteLine("✅ Ollama found! Starting Ollama service...");
            
            // Start Ollama serve in background
            var ollamaProcess = System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = "ollama",
                Arguments = "serve",
                UseShellExecute = false,
                CreateNoWindow = true
            });
            
            if (ollamaProcess != null)
            {
                Console.WriteLine("✅ Ollama service started on localhost:11434");
                Console.WriteLine("⚠️  Note: Make sure you have downloaded a model (e.g., ollama pull llama3.2:3b) for AI features to work.");
            }
        }
        else
        {
            Console.WriteLine("⚠️  Ollama not found. Project will run without AI features.");
            Console.WriteLine("   To enable AI features, install Ollama from: https://ollama.ai/download");
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine($"⚠️  Ollama check failed: {ex.Message}");
    Console.WriteLine("   Project will run without AI features.");
}

Console.WriteLine();
Console.WriteLine("🚀 Starting WasteNaut application...");

app.Run();
