using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data.Context;
using Data.Dtos.Category;
using Data.Entities.Category;
using Service.IServices;
using MongoDB.Driver;

namespace Service.Service
{
    public class CategoryService : ICategoryService
    {
        private readonly MongoDbContext _mongoDbContext;

        public CategoryService(MongoDbContext mongoDbContext)
        {
            _mongoDbContext = mongoDbContext;
        }

        public async Task<List<Category>> GetAllCategories()
        {
            return await _mongoDbContext.Categories.Find(_ => true).ToListAsync();
        }

        public async Task<Category> GetCategory(string id)
        {
            return await _mongoDbContext.Categories.Find(c => c.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Category> CreateCategory(CreateCategoryDTO createCategoryDTO)
        {
            var category = new Category
            {
                Name = createCategoryDTO.Name,
                Description = createCategoryDTO.Description
            };

            await _mongoDbContext.Categories.InsertOneAsync(category);
            return category;
        }

        // CategoryService.cs
        public async Task UpdateCategory(string id, CreateCategoryDTO createCategoryDTO)
        {
            var filter = Builders<Category>.Filter.Eq(c => c.Id, id);
            var update = Builders<Category>.Update
                .Set(c => c.Name, createCategoryDTO.Name)
                .Set(c => c.Description, createCategoryDTO.Description);

            await _mongoDbContext.Categories.UpdateOneAsync(filter, update);
        }


        public async Task DeleteCategory(string id)
        {
            await _mongoDbContext.Categories.DeleteOneAsync(c => c.Id == id);
        }
    }
}