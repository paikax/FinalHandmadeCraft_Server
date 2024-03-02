using System.Collections.Generic;
using System.Threading.Tasks;
using Data.Dtos.Category;
using Data.Entities.Category;

namespace Service.IServices
{
    public interface ICategoryService
    {
        Task<List<Category>> GetAllCategories();
        Task<Category> GetCategoryById(string id);
    }
}