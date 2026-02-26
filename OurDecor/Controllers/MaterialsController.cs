using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OurDecor.Data;
using OurDecor.Models;

namespace OurDecor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MaterialsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MaterialsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/materials
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetMaterials()
        {
            var materials = await _context.Materials
                .Include(m => m.MaterialType)
                .Select(m => new
                {
                    m.Id,
                    m.Name,
                    MaterialType = m.MaterialType.Name,
                    m.UnitPrice,
                    m.StockQuantity,
                    m.UnitOfMeasure
                })
                .ToListAsync();

            return Ok(materials);
        }

        // GET: api/materials/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetMaterial(int id)
        {
            var material = await _context.Materials
                .Include(m => m.MaterialType)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (material == null)
            {
                return NotFound(new { message = $"Материал с ID {id} не найден" });
            }

            var result = new
            {
                material.Id,
                material.Name,
                MaterialType = material.MaterialType.Name,
                material.MaterialTypeId,
                material.UnitPrice,
                material.StockQuantity,
                material.MinQuantity,
                material.PackageQuantity,
                material.UnitOfMeasure
            };

            return Ok(result);
        }

        // GET: api/materials/types
        [HttpGet("types")]
        public async Task<ActionResult<IEnumerable<MaterialType>>> GetMaterialTypes()
        {
            return await _context.MaterialTypes.ToListAsync();
        }
    }
}
