using System.Collections.Generic;
using System.Threading.Tasks;
using Data.Dtos.Material;
using Data.Entities.Material;

namespace Service.IServices
{
    public interface IMaterialService
    {
        Task<List<Material>> GetAllMaterials();
        Task<Material> GetMaterial(string id);
        Task<Material> CreateMaterial(CreateMaterialDTO createMaterialDTO);
        Task UpdateMaterial(string id, CreateMaterialDTO createMaterialDTO);
        Task DeleteMaterial(string id);
    }
}