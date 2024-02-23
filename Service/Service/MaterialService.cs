using System.Collections.Generic;
using System.Threading.Tasks;
using Data.Context;
using Data.Dtos.Material;
using Data.Entities.Material;
using MongoDB.Driver;
using Service.IServices;

namespace Service.Service
{
    public class MaterialService : IMaterialService
    {
        private readonly MongoDbContext _mongoDbContext;

        public MaterialService(MongoDbContext mongoDbContext)
        {
            _mongoDbContext = mongoDbContext;
        }

        public async Task<List<Material>> GetAllMaterials()
        {
            return await _mongoDbContext.Materials.Find(_ => true).ToListAsync();
        }

        public async Task<Material> GetMaterial(string id)
        {
            return await _mongoDbContext.Materials.Find(m => m.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Material> CreateMaterial(CreateMaterialDTO createMaterialDTO)
        {
            var material = new Material
            {
                Description = createMaterialDTO.Description,
                Price = createMaterialDTO.Price,
                Quantity = createMaterialDTO.Quantity,
                Images = createMaterialDTO.Images,
                CategoryOfMaterial = createMaterialDTO.CategoryOfMaterial
            };

            await _mongoDbContext.Materials.InsertOneAsync(material);
            return material;
        }

        public async Task UpdateMaterial(string id, CreateMaterialDTO createMaterialDTO)
        {
            var filter = Builders<Material>.Filter.Eq(m => m.Id, id);
            var update = Builders<Material>.Update
                .Set(m => m.Description, createMaterialDTO.Description)
                .Set(m => m.Price, createMaterialDTO.Price)
                .Set(m => m.Quantity, createMaterialDTO.Quantity)
                .Set(m => m.Images, createMaterialDTO.Images)
                .Set(m => m.CategoryOfMaterial, createMaterialDTO.CategoryOfMaterial);

            await _mongoDbContext.Materials.UpdateOneAsync(filter, update);
        }

        public async Task DeleteMaterial(string id)
        {
            await _mongoDbContext.Materials.DeleteOneAsync(m => m.Id == id);
        }
    }
}