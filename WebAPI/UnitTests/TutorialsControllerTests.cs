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
            Assert.Empty(tutorials); // Assuming no tutorials are returned in this case
        }
        
        [Fact]
        public async Task AddCommentToTutorial_ReturnsNoContent()
        {
            // Arrange
            string tutorialId = "tutorialId";
            var comment = new CommentCreateRequest();
            var tutorialServiceMock = new Mock<ITutorialService>();
            tutorialServiceMock.Setup(service => service.AddCommentToTutorial(tutorialId, comment))
                .Returns(Task.CompletedTask);

            var controller = new TutorialsController(tutorialServiceMock.Object);

            // Act
            var result = await controller.AddCommentToTutorial(tutorialId, comment);

            // Assert
            var noContentResult = Assert.IsType<NoContentResult>(result);
            Assert.Equal(204, noContentResult.StatusCode);
        }
    }
}