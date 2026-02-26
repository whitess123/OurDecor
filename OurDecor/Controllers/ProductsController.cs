using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OurDecor.Data;
using OurDecor.Models;

namespace OurDecor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProductsController (ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetProducts()
        {
            var products = await _context.Products
                .Include(p => p.ProductType)
                .Include(p => p.ProductMaterials)
                    .ThenInclude(pm => pm.Material)
                .ToListAsync();

            var result = products.Select(p => new
            {
                p.Id,
                p.Name,
                p.Article,
                ProductType = p.ProductType?.Name ?? "Неизвестно",
                p.MinPartnerPrice,
                p.RollWidth,
                // Расчет стоимости из материалов
                TotalCost = p.ProductMaterials.Sum(pm => pm.QuantityRequired * pm.Material.UnitPrice)
            });

            return Ok(result);
        }

        // GET: api/products/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetProduct(int id)
        {
            var product = await _context.Products
                .Include(p => p.ProductType)
                .Include(p => p.ProductMaterials)
                    .ThenInclude(pm => pm.Material)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound(new { message = $"Продукт с ID {id} не найден" });
            }

            var result = new
            {
                product.Id,
                product.Name,
                product.Article,
                ProductType = product.ProductType?.Name ?? "Неизвестно",
                product.ProductTypeId,
                product.MinPartnerPrice,
                product.RollWidth,
                TotalCost = product.ProductMaterials.Sum(pm => pm.QuantityRequired * pm.Material.UnitPrice)
            };

            return Ok(result);
        }

        // GET: api/products/{id}/materials
        [HttpGet("{id}/materials")]
        public async Task<ActionResult<object>> GetProductMaterials(int id)
        {
            var product = await _context.Products
                .Include(p => p.ProductMaterials)
                    .ThenInclude(pm => pm.Material)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound(new { message = $"Продукт с ID {id} не найден" });
            }

            var materials = product.ProductMaterials.Select(pm => new
            {
                pm.MaterialId,
                MaterialName = pm.Material.Name,
                pm.QuantityRequired,
                UnitPrice = pm.Material.UnitPrice,
                UnitOfMeasure = pm.Material.UnitOfMeasure,
                TotalPrice = pm.QuantityRequired * pm.Material.UnitPrice
            });

            return Ok(materials);
        }

        // POST: api/products
        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct(Product product)
        {
            if (product.MinPartnerPrice < 0)
            {
                return BadRequest(new { message = "Минимальная стоимость не может быть отрицательной" });
            }

            if (product.RollWidth < 0)
            {
                return BadRequest(new { message = "Ширина рулона не может быть отрицательной" });
            }

            var productType = await _context.ProductTypes.FindAsync(product.ProductTypeId);
            if (productType == null)
            {
                return BadRequest(new { message = "Указанный тип продукции не существует" });
            }

            var existingArticle = await _context.Products
                .FirstOrDefaultAsync(p => p.Article == product.Article);

            if (existingArticle != null)
            {
                return BadRequest(new { message = $"Продукт с артикулом '{product.Article}' уже существует" });
            }

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }

        // PUT: api/products/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, Product product)
        {
            var existingProduct = await _context.Products.FindAsync(id);
            if (existingProduct == null)
            {
                return NotFound(new { message = $"Продукт с ID {id} не найден" });
            }

            if (product.MinPartnerPrice < 0)
            {
                return BadRequest(new { message = "Минимальная стоимость не может быть отрицательной" });
            }

            if (product.RollWidth < 0)
            {
                return BadRequest(new { message = "Ширина рулона не может быть отрицательной" });
            }

            var productType = await _context.ProductTypes.FindAsync(product.ProductTypeId);
            if (productType == null)
            {
                return BadRequest(new { message = "Указанный тип продукции не существует" });
            }

            if (existingProduct.Article != product.Article)
            {
                var existingArticle = await _context.Products
                    .FirstOrDefaultAsync(p => p.Article == product.Article && p.Id != id);

                if (existingArticle != null)
                {
                    return BadRequest(new { message = $"Продукт с артикулом '{product.Article}' уже существует" });
                }
            }

            existingProduct.Name = product.Name;
            existingProduct.Article = product.Article;
            existingProduct.ProductTypeId = product.ProductTypeId;
            existingProduct.MinPartnerPrice = product.MinPartnerPrice;
            existingProduct.RollWidth = product.RollWidth;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return Ok(new { message = "Продукт успешно обновлен" });
        }

        // DELETE: api/products/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound(new { message = $"Продукт с ID {id} не найден" });
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Продукт успешно удален" });
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
