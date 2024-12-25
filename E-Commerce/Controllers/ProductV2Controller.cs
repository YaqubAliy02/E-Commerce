using E_Commerce.Data;
using E_Commerce.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace E_Commerce.Controllers
{
    //[Authorize]
    [ApiVersion("2.0")]
    [Route("api/v{version:ApiVersion}/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ECommerceDbContext _context;

        public ProductController(ECommerceDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<string> GetProducts()
        {
            return "This is version 2.0 of the Products API";
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _context.Products.Include(p =>
            p.Category).FirstOrDefaultAsync(p => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            return product;
        }

        [HttpPost]
        //[Authorize(Roles = "Admin")]
        public async Task<ActionResult<Product>> PostProduct(Product product)
        {
            if (string.IsNullOrEmpty(product.Name))
            {
                Log.Warning("Product creation failed due to missing name.");
                return BadRequest("Product name is required.");
            }

            try
            {
                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                Log.Information($"Product {product.Name} created with Id {product.Id} successfully.");
            }
            catch (Exception exception)
            {
                Log.Error(exception, "Failed to create product {ProductName}", product.Name);
                throw;
            }

            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Product>>> SearchProducts(
    [FromQuery] string? name,
    [FromQuery] int? categoryId,
    [FromQuery] decimal? minPrice,
    [FromQuery] decimal? maxPrice,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 10,
    [FromQuery] string? sortBy = null,
    [FromQuery] bool ascending = true
            )
        {
            var query = _context.Products.AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(p => p.Name.Contains(name));
            }
            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId);
            }
            if (minPrice.HasValue)
            {
                query = query.Where(p => p.Price >= minPrice.Value);
            }
            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= maxPrice.Value);
            }

            if (!string.IsNullOrEmpty(sortBy))
            {
                query = ascending
                ? query.OrderBy(p => EF.Property<object>(p, sortBy))
                : query.OrderByDescending(p => EF.Property<object>(p, sortBy));
            }

            // Get the total number of items before pagination
            var totalItems = await query.CountAsync();

            // Apply pagination and include category information
            var products = await query.Include(p => p.Category)
                .Skip((page - 1) * pageSize)  // Skip the items based on page number
                .Take(pageSize)  // Take the specified number of items
                .ToListAsync();

            // Return the paginated result along with metadata
            return Ok(new
            {
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling((double)totalItems / pageSize),  // Calculate total pages
                Page = page,
                PageSize = pageSize,
                Products = products
            });
        }


        [HttpPut("{id}")]
        // [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutProduct(int id, Product product)
        {
            if (id != product.Id)
            {
                return BadRequest();
            }

            _context.Entry(product).State = EntityState.Modified;

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
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }

        [HttpPost("{id}/upload-image")]
        public async Task<IActionResult> UploadImage(int id, IFormFile file)
        {

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var fileExtension = Path.GetExtension(file.FileName);

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound("Product not found.");
            }
            if (file == null || file.Length == 0)
            {
                return BadRequest("Invalid file.");
            }

            if (!allowedExtensions.Contains(fileExtension.ToLower()))
            {
                return BadRequest("Invalid file type. Only JPG, JPEG, PNG, and GIF are allowed.");
            }

            if (!string.IsNullOrEmpty(product.ImageUrl))
            {
                var oldImagePath = Path.Combine("wwwroot",
                product.ImageUrl.TrimStart('/'));
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
            }
            // Ensure the images directory exists
            var imagePath = Path.Combine("wwwroot", "images");
            if (!Directory.Exists(imagePath))
            {
                Directory.CreateDirectory(imagePath);
            }

            // Generate a unique file name and save the file
            var fileName =
             $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            var filePath = Path.Combine(imagePath, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            // Update the product's ImageUrl
            product.ImageUrl = $"/images/{fileName}";
            _context.Entry(product).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok(new { ImageUrl = product.ImageUrl });
        }
    }
}
