using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OurDecor.Data;

namespace OurDecor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CalculationController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CalculationController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: api/calculation/material-requirement
        [HttpPost("material-requirement")]
        public async Task<ActionResult<int>> CalculateMaterialRequirement(
           int productTypeId,
           int materialTypeId,
           int productQuantity,
           decimal parameter1,
           decimal parameter2,
           decimal stockQuantity)
        {
            try
            {
                if (productQuantity <= 0 || parameter1 <= 0 || parameter2 <= 0)
                {
                    return BadRequest(new { message = "Количество продукции и параметры должны быть положительными числами" });
                }

                if (stockQuantity < 0)
                {
                    return BadRequest(new { message = "Количество материала на складе не может быть отрицательным" });
                }

                var productType = await _context.ProductTypes.FindAsync(productTypeId);
                if (productType == null)
                {
                    return Ok(-1);
                }

                var materialType = await _context.MaterialTypes.FindAsync(materialTypeId);
                if (materialType == null)
                {
                    return Ok(-1);
                }

                // Расчет количества материала на одну единицу продукции
                decimal materialPerUnit = parameter1 * parameter2 * productType.Coefficient;

                // Общее необходимое количество материала
                decimal totalRequired = materialPerUnit * productQuantity;

                // Увеличение с учетом процента брака
                decimal withDefect = totalRequired * (1 + materialType.DefectPercentage);

                // Вычитаем наличие на складе
                decimal needed = withDefect - stockQuantity;

                if (needed <= 0)
                {
                    return Ok(0);
                }

                int result = (int)Math.Ceiling(needed);
                return Ok(result);
            }
            catch (Exception)
            {
                return Ok(-1);
            }
        }
    }
}
