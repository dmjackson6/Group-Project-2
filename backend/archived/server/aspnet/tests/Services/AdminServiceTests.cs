using Microsoft.EntityFrameworkCore;
using Moq;
using WasteNaut.Admin.Data;
using WasteNaut.Admin.DTOs;
using WasteNaut.Admin.Models;
using WasteNaut.Admin.Services;
using Xunit;

namespace WasteNaut.Admin.Tests.Services
{
    public class AdminServiceTests
    {
        private readonly DbContextOptions<WasteNautDbContext> _dbContextOptions;
        private readonly Mock<IAuditService> _mockAuditService;

        public AdminServiceTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<WasteNautDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _mockAuditService = new Mock<IAuditService>();
        }

        [Fact]
        public async Task LoginAsync_WithValidCredentials_ReturnsLoginResponse()
        {
            // Arrange
            using var context = new WasteNautDbContext(_dbContextOptions);
            var adminService = new AdminService(context, _mockAuditService.Object);

            // Add test data
            var role = new Role { Id = 1, Name = "Super Admin", Permissions = "[\"all\"]" };
            var admin = new Admin
            {
                Id = 1,
                Name = "Test Admin",
                Email = "admin@test.com",
                PasswordHash = "hashed_password",
                RoleId = 1,
                Status = "active"
            };

            context.Roles.Add(role);
            context.Admins.Add(admin);
            await context.SaveChangesAsync();

            var loginRequest = new LoginRequestDto
            {
                Email = "admin@test.com",
                Password = "password"
            };

            // Act
            var result = await adminService.LoginAsync(loginRequest);

            // Assert
            Assert.True(result.Success);
            Assert.NotEmpty(result.Token);
            Assert.Equal("Test Admin", result.User.Name);
        }

        [Fact]
        public async Task LoginAsync_WithInvalidCredentials_ThrowsUnauthorizedException()
        {
            // Arrange
            using var context = new WasteNautDbContext(_dbContextOptions);
            var adminService = new AdminService(context, _mockAuditService.Object);

            var loginRequest = new LoginRequestDto
            {
                Email = "nonexistent@test.com",
                Password = "password"
            };

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => adminService.LoginAsync(loginRequest));
        }

        [Fact]
        public async Task GetByIdAsync_WithValidId_ReturnsAdmin()
        {
            // Arrange
            using var context = new WasteNautDbContext(_dbContextOptions);
            var adminService = new AdminService(context, _mockAuditService.Object);

            var role = new Role { Id = 1, Name = "Super Admin", Permissions = "[\"all\"]" };
            var admin = new Admin
            {
                Id = 1,
                Name = "Test Admin",
                Email = "admin@test.com",
                PasswordHash = "hashed_password",
                RoleId = 1,
                Status = "active"
            };

            context.Roles.Add(role);
            context.Admins.Add(admin);
            await context.SaveChangesAsync();

            // Act
            var result = await adminService.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test Admin", result.Name);
        }

        [Fact]
        public async Task GetByIdAsync_WithInvalidId_ReturnsNull()
        {
            // Arrange
            using var context = new WasteNautDbContext(_dbContextOptions);
            var adminService = new AdminService(context, _mockAuditService.Object);

            // Act
            var result = await adminService.GetByIdAsync(999);

            // Assert
            Assert.Null(result);
        }
    }
}
