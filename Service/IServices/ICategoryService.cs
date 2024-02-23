using System.Collections.Generic;
using System.Threading.Tasks;
using Data.Dtos.Category;
using Data.Entities.Category;

namespace Service.IServices
{
    public interface ICategoryService
    {
        Task<List<Category>> GetAllCategories();
        Task<Category> GetCategory(string id);
        Task<Category> CreateCategory(CreateCategoryDTO createCategoryDTO);
        Task UpdateCategory(string id, CreateCategoryDTO createCategoryDTO);
        Task DeleteCategory(string id);
    }
}