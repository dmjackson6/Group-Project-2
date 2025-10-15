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

// Add Entity Framework
builder.Services.AddDbContext<WasteNautDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    ));

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
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
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
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// Add Services
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IOrganizationService, OrganizationService>();
builder.Services.AddScoped<IDonationService, DonationService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IMatchService, MatchService>();
builder.Services.AddScoped<IAuditService, AuditService>();

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

// Serve static files from frontend
app.UseDefaultFiles();
app.UseStaticFiles();

// Serve frontend HTML files
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "../../../frontend/html")),
    RequestPath = "/html"
});

// Serve frontend resources
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "../../../frontend/resources")),
    RequestPath = "/resources"
});

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

// Route root to frontend index.html
app.MapGet("/", () => Results.Redirect("/html/index.html"));

// Mock API endpoints for development
app.MapGet("/api/inventory", () => 
{
    var mockData = System.IO.File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "../../../mocks/donations.json"));
    return Results.Json(System.Text.Json.JsonSerializer.Deserialize<object>(mockData));
});

app.MapGet("/api/users", () => 
{
    var mockData = System.IO.File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "../../../mocks/users.json"));
    return Results.Json(System.Text.Json.JsonSerializer.Deserialize<object>(mockData));
});

app.MapGet("/api/requests", () => 
{
    var mockData = System.IO.File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "../../../mocks/matches.json"));
    return Results.Json(System.Text.Json.JsonSerializer.Deserialize<object>(mockData));
});

app.MapControllers();

// Auto-migrate database in development
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<WasteNautDbContext>();
        context.Database.EnsureCreated();
    }
}

app.Run();
