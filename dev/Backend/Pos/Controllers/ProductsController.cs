using Microsoft.AspNetCore.Mvc;
using Pos.Models;

namespace Pos.Controllers
{
    [Route("pos/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {

        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            // Simulación de datos
            var products = new List<object>
            {
                new { Id = 1, Name = "Product 1", Price = 10.99 },
                new { Id = 2, Name = "Product 2", Price = 15.99 }
            };

            return Ok(products);
        }

        [HttpGet("{productId:string}")]
        public async Task<IActionResult> GetProductById(string productId)
        {
            // Simulación de obtener un producto
            var product = new { Id = productId, Name = "Product " + id, Price = 12.99 };

            if (product == null)
            {
                return NotFound(new { Message = $"Product with ID {id} not found" });
            }

            return Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdProduct = new { Id = 3, Name = request.Name, Price = request.Price };

            return CreatedAtAction(nameof(GetProductById), new { id = createdProduct.Id }, createdProduct);
        }

        [HttpPut("{productId:string}")]
        public async Task<IActionResult> UpdateProduct(string productId, [FromBody] Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Simulación de actualización
            var updatedProduct = new { Id = id, Name = request.Name, Price = request.Price };

            return Ok(updatedProduct);
        }

        [HttpDelete("{productId:string}")]
        public async Task<IActionResult> DeleteProduct(string productId)
        {
            // Simulación de eliminación
            var deleted = true; // Supongamos que se eliminó correctamente

            if (!deleted)
            {
                return NotFound(new { Message = $"Product with ID {id} not found" });
            }

            return NoContent();
        }
    }
}
