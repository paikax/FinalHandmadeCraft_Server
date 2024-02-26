using System.Collections.Generic;
using System.Threading.Tasks;
using Data.Dtos.Tutorial;
using Data.Entities.Tutorial;

namespace Service.IServices
{
    public interface ITutorialService
    {
        // Task<List<TutorialDTO>> GetAllTutorials();
        // Task<TutorialDTO> GetTutorial(string id);
        public Task<TutorialDTO> GetTutorialById(string id);
        Task<TutorialDTO> CreateTutorial(TutorialCreateRequest model);
        Task UpdateTutorial(string id, TutorialDTO tutorialDTO); 
        Task DeleteTutorial(string id);
        Task AddCommentToTutorial(string tutorialId, CommentCreateRequest comment);
        Task AddLikeToTutorial(string tutorialId, LikeDTO like);
        Task RemoveCommentFromTutorial(string tutorialId, string commentId);
        Task RemoveLikeFromTutorial(string tutorialId, string likeId, string userId);
        Task<List<TutorialDTO>> GetAllTutorials();
        Task<List<TutorialDTO>> SearchTutorials(string searchValue);
        
    }
}