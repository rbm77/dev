using Microsoft.AspNetCore.Mvc;
using Pos.Interfaces;
using Pos.Models;
using static Pos.Utilities.Enums;

namespace Pos.Controllers
{
    [Route("pos/[controller]")]
    [ApiController]
    public class ProductsController(IProductService productService, ILogManager logManager) : ControllerBase
    {

        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            try
            {
                List<Product> products = await productService.GetAllProducts();
                return Ok(products);
            }
            catch
            {
                return StatusCode(500);
            }
        }

        [HttpGet("{productId:string}")]
        public async Task<IActionResult> GetProductById(string productId)
        {
            try
            {
                if (string.IsNullOrEmpty(productId))
                {
                    return BadRequest();
                }
                Product? product = await productService.GetProductById(productId);
                if (product == null)
                {
                    return NotFound();
                }
                return Ok(product);
            }
            catch
            {
                return StatusCode(500);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] Product product)
        {
            try
            {
                if (product == null)
                {
                    return BadRequest();
                }
                string productId = await productService.CreateProduct(product);
                if (string.IsNullOrEmpty(productId))
                {
                    _ = logManager.Log("Error creating product. Empty ID.", LogType.Error);
                    return StatusCode(500);
                }
                return CreatedAtAction(nameof(CreateProduct), productId);
            }
            catch
            {
                return StatusCode(500);
            }
        }

        [HttpPut("{productId:string}")]
        public async Task<IActionResult> UpdateProduct(string productId, [FromBody] Product product)
        {
            try
            {
                if (string.IsNullOrEmpty(productId) || product == null)
                {
                    return BadRequest();
                }
                bool updated = await productService.UpdateProduct(productId, product);
                return updated ? NoContent() : NotFound();
            }
            catch
            {
                return StatusCode(500);
            }
        }

        [HttpDelete("{productId:string}")]
        public async Task<IActionResult> DeleteProduct(string productId)
        {
            try
            {
                if (string.IsNullOrEmpty(productId))
                {
                    return BadRequest();
                }
                bool deleted = await productService.DeleteProduct(productId);
                return deleted ? NoContent() : NotFound();
            }
            catch
            {
                return StatusCode(500);
            }
        }
    }
}
