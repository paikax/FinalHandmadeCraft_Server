using AutoMapper;
using Data.Dtos.Category;
using Data.Dtos.Tutorial;
using Data.Entities.Category;
using Data.Entities.Comment;
using Data.Entities.Order;
using Data.Entities.Tutorial;
using Data.Entities.User;
using Data.ViewModels.User;

namespace WebAPI.Mapper
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMap()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                // ---
                
                // category
                config.CreateMap<CreateCategoryDTO, Category>().ReverseMap();
                
                // tutorial
                config.CreateMap<TutorialCreateRequest, Tutorial>()
                    .ForMember(dest => dest.Comments, opt => opt.Ignore()) // Exclude Comments mapping for TutorialCreateRequest
                    .ReverseMap();

                config.CreateMap<TutorialDTO, Tutorial>().ReverseMap();
                config.CreateMap<Tutorial, TutorialDTO>()
                    .ForMember(dest => dest.Comments, opt => opt.MapFrom(src => src.Comments))
                    .ForMember(dest => dest.Likes, opt => opt.MapFrom(src => src.Likes))
                    .ReverseMap();
                

                // Comment and Like mappings
                config.CreateMap<CommentCreateRequest, Comment>().ReverseMap();
                config.CreateMap<CommentDTO, Comment>().ReverseMap();
                config.CreateMap<LikeDTO, Like>().ReverseMap();
                config.CreateMap<LikeResponse, Like>().ReverseMap();
                
                // Order mappings
                config.CreateMap<Order, OrderDto>().ReverseMap();
                config.CreateMap<OrderItem, OrderItemDto>().ReverseMap();
                config.CreateMap<OrderRequest, Order>();
                
                // user 
                config.CreateMap<RegisterRequest, User>()
                    .ForMember(dest => dest.EmailConfirmed, act => act.Ignore())
                    .ReverseMap()
                    .ForMember(dest => dest.VerificationToken, act => act.MapFrom(src => src.VerificationToken));
                
                config.CreateMap<AuthenticationRequest, User>().ReverseMap();
                config.CreateMap<AuthenticationResponse, User>().ReverseMap();
                config.CreateMap<UpdateRequest, User>().ReverseMap();
                config.CreateMap<UpdateRequest, User>()
                    .ForAllMembers(x=>x.Condition(
                        (src, dest, prop) =>
                        {
                            if (prop == null) return false;
                            if (prop.GetType() == typeof(string) && string.IsNullOrEmpty((string)prop)) return false;
                            
                            return true;
                        }));
            });
            return mappingConfig;
        }
    }
}