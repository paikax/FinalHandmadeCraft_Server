using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data.Dtos.Tutorial;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Service.IServices;
using WebAPI.Controllers;
using Xunit;

namespace WebAPI.UnitTests
{
    public class TutorialsControllerTests
    {
        [Fact]
        public async Task GetTutorials_ReturnsOkResultWithTutorials()
        {
            // Arrange
            var tutorialServiceMock = new Mock<ITutorialService>();
            tutorialServiceMock.Setup(service => service.GetAllTutorials())
                .ReturnsAsync(new List<TutorialDTO>());

            var controller = new TutorialsController(tutorialServiceMock.Object);

            // Act
            var result = await controller.GetTutorials();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var tutorials = Assert.IsAssignableFrom<List<TutorialDTO>>(okResult.Value);
            Assert.Empty(tutorials);
        }
        
        [Fact]
        public async Task AddCommentToTutorial_ReturnsOkResult()
        {
            // Arrange
            var tutorialId = "6621ca769c6e1a327560468e";
            var comment = new CommentCreateRequest 
            { 
                Content = "This is a test comment phong",
                TimeStamp = DateTime.Now,
                UserId = "06084eaf-a70f-42b3-9bd7-40a9e6b955f1" 
            };
            var tutorialServiceMock = new Mock<ITutorialService>();
            tutorialServiceMock.Setup(service => service.AddCommentToTutorial(tutorialId, comment))
                .Returns(Task.CompletedTask);

            var controller = new TutorialsController(tutorialServiceMock.Object);

            // Act
            var result = await controller.AddCommentToTutorial(tutorialId, comment);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("You've just added new comment", okResult.Value);
        }

        [Fact]
        public async Task RemoveCommentFromTutorial_ReturnsOkResult()
        {
            // Arrange
            var tutorialId = "6621ca769c6e1a327560468e";
            var commentId = "06084eaf-a70f-42b3-9bd7-40a9e6b955f1";
            var tutorialServiceMock = new Mock<ITutorialService>();
            tutorialServiceMock.Setup(service => service.RemoveCommentFromTutorial(tutorialId, commentId))
                .Returns(Task.CompletedTask);

            var controller = new TutorialsController(tutorialServiceMock.Object);

            // Act
            var result = await controller.RemoveCommentFromTutorial(tutorialId, commentId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("You've removed the comment", okResult.Value);
        }
        
        [Fact]
        public async Task AddLikeToTutorial_ReturnsOkResult()
        {
            // Arrange
            var tutorialId = "6621ca769c6e1a327560468e";
            var like = new LikeDTO 
            { 
                UserId = "06084eaf-a70f-42b3-9bd7-40a9e6b955f1", 
                TimeStamp = DateTime.Now
            };
            var tutorialServiceMock = new Mock<ITutorialService>();
            tutorialServiceMock.Setup(service => service.AddLikeToTutorial(tutorialId, like))
                .Returns(Task.CompletedTask);
        
            var controller = new TutorialsController(tutorialServiceMock.Object);
        
            // Act
            var result = await controller.AddLikeToTutorial(tutorialId, like);
        
            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Like added successfully", ((dynamic)okResult.Value).message.ToString());
        }

        [Fact]
        public async Task RemoveLikeFromTutorial_ReturnsOkResult()
        {
            // Arrange
            var tutorialId = "6621ca769c6e1a327560468e";
            var likeId = "662a10acdb0efbc3a341d7bb";
            var userId = "06084eaf-a70f-42b3-9bd7-40a9e6b955f1"; // Provide a valid user ID for testing
            var tutorialServiceMock = new Mock<ITutorialService>();
            tutorialServiceMock.Setup(service => service.RemoveLikeFromTutorial(tutorialId, likeId, userId))
                .Returns(Task.CompletedTask);
        
            var controller = new TutorialsController(tutorialServiceMock.Object);
        
            // Act
            var result = await controller.RemoveLikeFromTutorial(tutorialId, likeId, userId);
        
            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Removed comment successfully!", okResult.Value);
        }
        
        
    }
}