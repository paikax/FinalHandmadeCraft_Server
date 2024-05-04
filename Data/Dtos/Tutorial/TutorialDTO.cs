using System;
using System.Collections.Generic;
using Data.ViewModels.User;

namespace Data.Dtos.Tutorial
{
    public class TutorialDTO
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string DifficultLevel { get; set; }
        public string CompletionTime { get; set; }
        public string Instruction { get; set; }
        public string VideoUrl { get; set; }
        public string CategoryId { get; set; }
        public string CategoryName { get; set; }
        
        public string Material { get; set; }
        public List<string> MaterialIds { get; set; } 
        public decimal Price { get; set; }
        public string UserProfilePicture { get; set; }
        public string UserName { get; set; }
        public string CreatorPayPalEmail { get; set; }
        
        public string CreatorPayPalFirstName { get; set; }
        public string CreatorPayPalLastName { get; set; }
        public string CreatedById { get; set; }
        public DateTime UploadTime { get; set; }
        public string CreatorEmail { get; set; }
        public List<LikeResponse> Likes { get; set; } = new List<LikeResponse>();
        public List<CommentDTO> Comments { get; set; } = new List<CommentDTO>();
        public int Quantity { get; set; }
        
    }
}