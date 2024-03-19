using System.Collections.Generic;
using System.Threading.Tasks;
using Data.Dtos.Comment;
using Data.Dtos.Tutorial;
using Data.Entities.Tutorial;

namespace Service.IServices
{
    public interface ITutorialService
    {
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
        Task AddReplyToComment(string tutorialId, string commentId, ReplyCreateRequest reply);
        
        Task RemoveReplyFromComment(string tutorialId, string commentId, string replyId);
    }
}