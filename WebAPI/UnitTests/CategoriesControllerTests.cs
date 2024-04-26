// using System.Collections.Generic;
// using System.Threading.Tasks;
// using Data.Dtos.Category;
// using Data.Entities.Category;
// using Microsoft.AspNetCore.Mvc;
// using Moq;
// using Service.IServices;
// using WebAPI.Controllers;
// using Xunit;
//
// namespace WebAPI.UnitTests
// {
//     public class CategoriesControllerTests
//     {
//         [Fact]
//         public async Task GetAllCategories_ReturnsOkResult()
//         {
//             // Arrange
//             var categories = new List<Category>(); 
//             var categoryServiceMock = new Mock<ICategoryService>();
//             categoryServiceMock.Setup(repo => repo.GetAllCategories()).ReturnsAsync(categories);
//             var controller = new CategoriesController(categoryServiceMock.Object);
//
//             // Act
//             var result = await controller.GetAllCategories();
//
//             // Assert
//             var okResult = Assert.IsType<OkObjectResult>(result.Result);
//             var returnedCategories = Assert.IsAssignableFrom<List<CategoryDTO>>(okResult.Value);
//             Assert.Equal(2, returnedCategories.Count); // Assert that the returned list contains 2 categories
//         }
//
//         
//     }
// }