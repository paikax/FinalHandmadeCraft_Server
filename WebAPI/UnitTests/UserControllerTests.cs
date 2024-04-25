using System;
using System.Threading.Tasks;
using AutoMapper;
using Common.Constants;
using Data.Entities.User;
using Data.ViewModels.User;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Moq;
using Service.IServices;
using WebAPI.Controllers;
using Xunit;

namespace WebAPI.UnitTests
{
    public class UserControllerTests
    {
        [Fact]
        public async Task CheckEmail_ReturnsOkResult()
        {
            // Arrange
            var email = "paika2060@gmail.com";
            var userServiceMock = new Mock<IUserService>();
            userServiceMock.Setup(repo => repo.EmailExists(email)).ReturnsAsync(true);
            var controller = new UserController(userServiceMock.Object, null, null, null, null);

            // Act
            var result = await controller.CheckEmail(email);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.True((bool)okResult.Value.GetType().GetProperty("EmailExists").GetValue(okResult.Value));
        }

        // [Fact]
        // public async Task Authenticate_WithCorrectCredentials_ReturnsExpectedUser()
        // {
        //     // Arrange
        //     var authenticationRequest = new AuthenticationRequest
        //     {
        //         Email = "paika2060@gmail.com",
        //         Password = "Phong123"
        //     };
        //
        //     var expectedUser = new User
        //     {
        //         Id = "06084eaf-a70f-42b3-9bd7-40a9e6b955f1",
        //         Email = "paika2060@gmail.com",
        //         FirstName = "Paika",
        //         LastName = "Sherwin"
        //     };
        //
        //     var mockUserService = new Mock<IUserService>();
        //     mockUserService.Setup(repo => repo.Authenticate(authenticationRequest, It.IsAny<string>()))
        //         .ReturnsAsync(new AuthenticationResponse(expectedUser, "jwtToken", "refreshToken"));
        //
        //     var controller = new UserController(mockUserService.Object, null, null, null, null);
        //
        //     // Act
        //     var result = await controller.Authenticate(authenticationRequest) as OkObjectResult;
        //
        //     // Assert
        //     Assert.NotNull(result);
        //
        //     var response = result.Value as AuthenticationResponse;
        //     Assert.NotNull(response);
        //
        //     Assert.Equal(expectedUser.Id, response.Id);
        //     Assert.Equal(expectedUser.Email, response.Email);
        //     Assert.Equal(expectedUser.FirstName, response.FirstName);
        //     Assert.Equal(expectedUser.LastName, response.LastName);
        // }
    }
}