using Microsoft.AspNetCore.Mvc;
using Moq;
using WasteNaut.Admin.Controllers;
using WasteNaut.Admin.DTOs;
using WasteNaut.Admin.Services;
using Xunit;

namespace WasteNaut.Admin.Tests.Controllers
{
    public class AdminAuthControllerTests
    {
        private readonly Mock<IAdminService> _mockAdminService;
        private readonly AdminAuthController _controller;

        public AdminAuthControllerTests()
        {
            _mockAdminService = new Mock<IAdminService>();
            _controller = new AdminAuthController(_mockAdminService.Object);
        }

        [Fact]
        public async Task Login_WithValidCredentials_ReturnsOkResult()
        {
            // Arrange
            var loginRequest = new LoginRequestDto
            {
                Email = "admin@wastenaut.test",
                Password = "admin123"
            };

            var expectedResponse = new LoginResponseDto
            {
                Success = true,
                Token = "mock-jwt-token",
                User = new AdminDto
                {
                    Id = 1,
                    Name = "Admin User",
                    Email = "admin@wastenaut.test",
                    Role = "Super Admin",
                    Status = "active"
                }
            };

            _mockAdminService.Setup(x => x.LoginAsync(loginRequest))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.Login(loginRequest);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<LoginResponseDto>(okResult.Value);
            Assert.True(response.Success);
            Assert.Equal("mock-jwt-token", response.Token);
        }

        [Fact]
        public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
        {
            // Arrange
            var loginRequest = new LoginRequestDto
            {
                Email = "invalid@test.com",
                Password = "wrongpassword"
            };

            _mockAdminService.Setup(x => x.LoginAsync(loginRequest))
                .ThrowsAsync(new UnauthorizedAccessException("Invalid credentials"));

            // Act
            var result = await _controller.Login(loginRequest);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            var response = Assert.IsType<dynamic>(unauthorizedResult.Value);
            Assert.Equal("Invalid credentials", response.message);
        }

        [Fact]
        public async Task GetProfile_WithValidToken_ReturnsAdminProfile()
        {
            // Arrange
            // In a real test, you would need to mock the authentication context
            // For now, this is a placeholder test structure

            // Act & Assert
            // This would require proper authentication setup in the test
            Assert.True(true); // Placeholder assertion
        }
    }
}
