using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Data.Context;
using Data.Dtos.Tutorial;
using Data.Entities.Comment;
using Data.Entities.Tutorial;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;
using Service.IServices;

namespace Service.Service
{
    public class TutorialService : ITutorialService
    {
        private readonly MongoDbContext _mongoDbContext;
        private readonly IMapper _mapper;
        private readonly AppDbContext _userDbContext;

        public TutorialService(MongoDbContext mongoDbContext, IMapper mapper, AppDbContext userDbContext)
        {
            _mongoDbContext = mongoDbContext;
            _mapper = mapper;
            _userDbContext = userDbContext;
        }

        public async Task<List<TutorialDTO>> GetAllTutorials()
        {
            var tutorials = await _mongoDbContext.Tutorials.Find(_ => true).ToListAsync();
            var tutorialsDTO = _mapper.Map<List<TutorialDTO>>(tutorials);

            foreach(var tutorial in tutorialsDTO)
            {
                var user = await _userDbContext.Users.FirstOrDefaultAsync(u => u.Id == tutorial.CreatedById);
                if(user != null)
                {
                    tutorial.UserName = $"{user.FirstName} {user.LastName}";
                    tutorial.UserProfilePicture = user.ProfilePhoto;
                }
            }

            return tutorialsDTO;
        }


        public async Task<TutorialDTO> GetTutorialById(string id)
        {
            var tutorial = await _mongoDbContext.Tutorials.Find(t => t.Id == id).FirstOrDefaultAsync();
            if (tutorial == null)
            {
                return null; // or handle appropriately if tutorial not found
            }

            var tutorialDTO = _mapper.Map<TutorialDTO>(tutorial);

            // Fetch user details
            var user = await _userDbContext.Users.FirstOrDefaultAsync(u => u.Id == tutorial.CreatedById);
            if (user != null)
            {
                tutorialDTO.CreatorPayPalEmail = user.PayPalEmail;
                tutorialDTO.CreatorPayPalFirstName = user.PayPalFirstName;
                tutorialDTO.CreatorPayPalLastName = user.PayPalLastName;
                tutorialDTO.UserName = $"{user.FirstName} {user.LastName}";
                tutorialDTO.UserProfilePicture = user.ProfilePhoto;
            }

            return tutorialDTO;
        }


        public async Task<TutorialDTO> CreateTutorial(TutorialCreateRequest model)
        {
            var tutorial = _mapper.Map<Tutorial>(model);
            tutorial.UploadTime = DateTime.UtcNow;

            // Exclude Comments mapping for TutorialCreateRequest
            tutorial.Comments = new List<Comment>();

            await _mongoDbContext.Tutorials.InsertOneAsync(tutorial);
            return _mapper.Map<TutorialDTO>(tutorial);
        }

        public Task UpdateTutorial(string id, TutorialDTO tutorialDTO)
        {
            throw new System.NotImplementedException();
        }

        // public async Task UpdateTutorial(string id, TutorialUpdateRequest model)
        // {
        //     var tutorial = _mapper.Map<Tutorial>(model);
        //     await _mongoDbContext.Tutorials.ReplaceOneAsync(t => t.Id == id, tutorial);
        // }

        public async Task DeleteTutorial(string id)
        {
            await _mongoDbContext.Tutorials.DeleteOneAsync(t => t.Id == id);
        }
        
        public async Task AddCommentToTutorial(string tutorialId, CommentCreateRequest comment) 
        {
            var mappedComment = _mapper.Map<Comment>(comment);
            mappedComment.Id = ObjectId.GenerateNewId().ToString();
            mappedComment.TimeStamp = DateTime.UtcNow; 
  
            var tutorialFilter = Builders<Tutorial>.Filter.Eq(t => t.Id, tutorialId);
            var tutorialUpdate = Builders<Tutorial>.Update.Push(t => t.Comments, mappedComment);
  
            await _mongoDbContext.Tutorials.UpdateOneAsync(tutorialFilter, tutorialUpdate);
        }

        public async Task AddLikeToTutorial(string tutorialId, LikeDTO like)
        {
            try
            {
                var mappedLike = _mapper.Map<Like>(like);
                mappedLike.Id = ObjectId.GenerateNewId().ToString();
                mappedLike.TimeStamp = DateTime.UtcNow;

                var tutorialFilter = Builders<Tutorial>.Filter.Eq(t => t.Id, tutorialId);
                var tutorialUpdate = Builders<Tutorial>.Update.Push(t => t.Likes, mappedLike);

                await _mongoDbContext.Tutorials.UpdateOneAsync(tutorialFilter, tutorialUpdate);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error adding like: {ex.Message}");
                throw ex;
            }
        }


        public async Task RemoveCommentFromTutorial(string tutorialId, string commentId)
        {
            var tutorialFilter = Builders<Tutorial>.Filter.Eq(t => t.Id, tutorialId);
            var commentFilter = Builders<Comment>.Filter.Eq(c => c.Id, commentId);
            var tutorialUpdate = Builders<Tutorial>.Update.PullFilter(t => t.Comments, commentFilter);

            await _mongoDbContext.Tutorials.UpdateOneAsync(tutorialFilter, tutorialUpdate);
        }

        public async Task RemoveLikeFromTutorial(string tutorialId, string likeId, string userId)
        {
            try
            {
                // Check if the like exists and belongs to the user
                var existingLike = await _mongoDbContext.Tutorials
                    .Find(t => t.Id == tutorialId && t.Likes.Any(like => like.Id == likeId))
                    .FirstOrDefaultAsync();


                if (existingLike != null)
                {
                    // Remove the like
                    var likeFilter = Builders<Like>.Filter.Eq(l => l.Id, likeId);
                    var update = Builders<Tutorial>.Update.PullFilter(t => t.Likes, likeFilter);

                    await _mongoDbContext.Tutorials.UpdateOneAsync(t => t.Id == tutorialId, update);
                }
                else
                {
                    // Handle the case where the like doesn't exist or doesn't belong to the user
                    throw new Exception("Invalid like or user");
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error removing like: {ex.Message}");
                throw ex;
            }
        }

        public async Task<List<TutorialDTO>> SearchTutorials(string searchTerm)
        {
            var tutorials = await _mongoDbContext.Tutorials
                .Find(t => t.Title.ToLower().Contains(searchTerm))
                .ToListAsync();
            var tutorialsDTO = _mapper.Map<List<TutorialDTO>>(tutorials);

            
            foreach(var tutorial in tutorialsDTO)
            {
                var user = await _userDbContext.Users.FirstOrDefaultAsync(u => u.Id == tutorial.CreatedById);
                if(user != null)
                {
                    tutorial.UserName = $"{user.FirstName} {user.LastName}";
                    tutorial.UserProfilePicture = user.ProfilePhoto;
                    tutorial.CreatorPayPalEmail = user.PayPalEmail;
                    tutorial.CreatorPayPalFirstName = user.PayPalFirstName;
                    tutorial.CreatorPayPalLastName = user.PayPalLastName;
                }
            }

            return tutorialsDTO;
        }

        
    }
}