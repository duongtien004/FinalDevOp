using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoeStoreBackend.DTOs;
using ShoeStoreBackend.Services;
using System.Threading.Tasks;

namespace ShoeStoreBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        // GET: /api/products?search=giày
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? search)
        {
            var products = await _productService.GetAllAsync(search);
            return Ok(products);
        }

        // GET: /api/products/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound(new { message = "Sản phẩm không tồn tại." });
            }
            return Ok(product);
        }

        // POST: /api/products
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] ProductDto productDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Dữ liệu không hợp lệ." });
            }

            try
            {
                var createdProduct = await _productService.CreateAsync(productDto);
                return CreatedAtAction(nameof(GetById), new { id = createdProduct.Id }, createdProduct);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        // PUT: /api/products/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] ProductDto productDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Dữ liệu không hợp lệ." });
            }

            try
            {
                var updatedProduct = await _productService.UpdateAsync(id, productDto);
                if (updatedProduct == null)
                {
                    return NotFound(new { message = "Sản phẩm không tồn tại." });
                }
                return Ok(updatedProduct);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        // DELETE: /api/products/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _productService.DeleteAsync(id);
                if (!result)
                {
                    return NotFound(new { message = "Sản phẩm không tồn tại." });
                }
                return NoContent();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }
    }
}