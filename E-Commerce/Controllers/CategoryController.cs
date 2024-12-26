using AutoMapper;
using E_Commerce.Data;
using E_Commerce.DTOs.Category;
using E_Commerce.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
   // [Authorize]
    public class CategoriesController : ControllerBase
    {
        private readonly ECommerceDbContext _context;
        private readonly IMapper mapper;
        public CategoriesController(ECommerceDbContext context, IMapper mapper)
        {
            _context = context;
            this.mapper = mapper;
        }
        // GET: api/Categories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetCategories()
        {
            var categories = await _context.Categories.ToListAsync();
            return Ok(this.mapper.Map<IEnumerable<CategoryDTO>>(categories));
        }

        // GET: api/Categories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryDTO>> GetCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return this.mapper.Map<CategoryDTO>(category);
        }

        // POST: api/Categories
        // [Authorize(Roles = "Admin")]
        public async Task<ActionResult<CategoryDTO>> PostCategory(CreateCategoryDTO categoryDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var category = this.mapper.Map<Category>(categoryDto);
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, this.mapper.Map<CategoryDTO>(category));
        }

        // PUT: api/Categories/5
        // [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutCategory(int id, UpdateCategoryDTO categoryDto)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            this.mapper.Map(categoryDto, category);  // Updates category with dto data

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(id))
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

        // DELETE: api/Categories/5
        // [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(c => c.Id == id);
        }
    }
}

