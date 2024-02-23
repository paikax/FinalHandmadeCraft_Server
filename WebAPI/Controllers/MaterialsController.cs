// MaterialsController.cs
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data.Dtos.Material;
using Data.Entities.Material;
using Service.IServices;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MaterialsController : ControllerBase
    {
        private readonly IMaterialService _materialService;

        public MaterialsController(IMaterialService materialService)
        {
            _materialService = materialService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Material>>> GetMaterials()
        {
            var materials = await _materialService.GetAllMaterials();
            return Ok(materials);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Material>> GetMaterial(string id)
        {
            var material = await _materialService.GetMaterial(id);

            if (material == null)
            {
                return NotFound();
            }

            return Ok(material);
        }

        [HttpPost]
        public async Task<ActionResult<Material>> PostMaterial(CreateMaterialDTO createMaterialDTO)
        {
            var createdMaterial = await _materialService.CreateMaterial(createMaterialDTO);
            return CreatedAtAction(nameof(GetMaterial), new { id = createdMaterial.Id }, createdMaterial);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutMaterial(string id, CreateMaterialDTO createMaterialDTO)
        {
            await _materialService.UpdateMaterial(id, createMaterialDTO);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMaterial(string id)
        {
            await _materialService.DeleteMaterial(id);
            return NoContent();
        }
    }
}