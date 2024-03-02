using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Data.Context;
using Data.Dtos.Category;
using Data.Entities.Category;
using Microsoft.EntityFrameworkCore;
using Service.IServices;
using MongoDB.Driver;

namespace Service.Service
{
    public class CategoryService : ICategoryService
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;

        public CategoryService(AppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<List<Category>> GetAllCategories()
        {
            var categories = await _dbContext.Categories.ToListAsync();
            return categories;
        }

        public async Task<Category> GetCategoryById(string id)
        {
            var category = await _dbContext.Categories.FirstOrDefaultAsync(c => c.Id == id);
            return category;
        }
        
    }
}