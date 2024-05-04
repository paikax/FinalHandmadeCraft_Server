using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data.Dtos.Comment;
using Data.Dtos.Tutorial;
using Data.Entities.Comment;
using Data.Entities.Tutorial;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using SendGrid.Helpers.Errors.Model;
using Service.IServices;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TutorialsController : ControllerBase
    {
        private readonly ITutorialService _tutorialService;

        public TutorialsController(ITutorialService tutorialService)
        {
            _tutorialService = tutorialService;
        }

        // [Authorize]
        [HttpGet]
        public async Task<ActionResult<List<TutorialDTO>>> GetTutorials()
        {
            var tutorials = await _tutorialService.GetAllTutorials();
            return Ok(tutorials);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TutorialDTO>> GetTutorial(string id)
        {
            var tutorial = await _tutorialService.GetTutorialById(id);

            if (tutorial == null)
            {
                return NotFound();
            }

            return Ok(tutorial);
        }

        [HttpPost]
        public async Task<ActionResult<TutorialDTO>> PostTutorial([FromBody] TutorialCreateRequest model)
        {
            var createdTutorial = await _tutorialService.CreateTutorial(model);
            return Ok(createdTutorial);
        }
        
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTutorial(string id, [FromBody] TutorialUpdateRequest model)
        {
            try
            {
                await _tutorialService.UpdateTutorial(id, model);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTutorial(string id)
        {
            await _tutorialService.DeleteTutorial(id);

            return NoContent();
        }

        [HttpPost("{tutorialId}/comments")]
        public async Task<IActionResult> AddCommentToTutorial(string tutorialId, [FromBody] CommentCreateRequest comment)
        {
            await _tutorialService.AddCommentToTutorial(tutorialId, comment);
            return Ok("You've just added new comment");
        }

        [HttpPost("{tutorialId}/likes")]
        public async Task<IActionResult> AddLikeToTutorial(string tutorialId, [FromBody] LikeDTO like)
        {
            try
            {
                await _tutorialService.AddLikeToTutorial(tutorialId, like);
                // Return a JSON response with a success message
                return Ok(new { message = "Like added successfully" });
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error adding like: {ex.Message}");
                // Return a JSON response with an error message
                return StatusCode(500, new { error = "Internal Server Error" });
            }
        }


        [HttpDelete("{tutorialId}/comments/{commentId}")]
        public async Task<IActionResult> RemoveCommentFromTutorial(string tutorialId, string commentId)
        {
            await _tutorialService.RemoveCommentFromTutorial(tutorialId, commentId);
            return Ok("You've removed the comment");
        }

        [HttpDelete("{tutorialId}/likes/{likeId}")]
        public async Task<IActionResult> RemoveLikeFromTutorial(string tutorialId, string likeId, string userId)
        {
            await _tutorialService.RemoveLikeFromTutorial(tutorialId, likeId, userId);
            return Ok("Removed comment successfully!");
        }
        
        [HttpGet("search")]
        public async Task<ActionResult<List<TutorialDTO>>> SearchTutorials(string searchTerm)
        {
            var tutorials = await _tutorialService.SearchTutorials(searchTerm);
            return Ok(tutorials);
        }
        
        [HttpPost("{tutorialId}/comments/{commentId}/replies")]
        public async Task<IActionResult> AddReplyToComment(string tutorialId, string commentId, [FromBody] ReplyCreateRequest reply)
        {
            try
            {
                await _tutorialService.AddReplyToComment(tutorialId, commentId, reply);
                return Ok("You've added new reply to the comment");
            }
            catch (Exception ex)    
            {
                Console.Error.WriteLine($"Error adding reply: {ex.Message}");
                return StatusCode(500, new { error = "Internal Server Error" });
            }
        }

        [HttpDelete("{tutorialId}/comments/{commentId}/replies/{replyId}")]
        public async Task<IActionResult> RemoveReplyFromComment(string tutorialId, string commentId, string replyId)
        {
            try
            {
                await _tutorialService.RemoveReplyFromComment(tutorialId, commentId, replyId);
                return Ok("You've just replied the comment");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error removing reply: {ex.Message}");
                return StatusCode(500, new { error = "Internal Server Error" });
            }
        }

    }
}