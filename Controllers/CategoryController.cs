using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;

namespace Shop.Controllers
{
    [Route("v1/categories")]
    public class CategoryController : ControllerBase
    {
        [HttpGet]
        [AllowAnonymous]
        [ResponseCache(VaryByHeader = "User-Agent", Location = ResponseCacheLocation.Any, Duration = 30)]
        public async Task<ActionResult<List<Category>>> GetAllCategories(
            [FromServices] DataContext context
        )
        {
            try
            {
                var categories = await context.Categories
                .AsNoTracking()
                .ToListAsync();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex });
            }
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<Category>> GetCategoryById(
            int id,
            [FromServices] DataContext context
        )
        {
            try
            {
                var category = await context.Categories
                    .AsNoTracking()
                    .SingleOrDefaultAsync(c => c.Id == id);

                if (category is null)
                    return NotFound(new { message = "Categoria não encontrada" });

                return Ok(category);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex });
            }
        }

        [HttpPost]
        [Authorize(Roles = "employee, manager")]
        public async Task<ActionResult<Category>> CreateCategory(
            [FromBody] Category category,
            [FromServices] DataContext context)
        {
            ActionResult<Category> result = null;

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                context.Categories.Add(category);
                await context.SaveChangesAsync();

                result = Ok(category);
            }
            catch (Exception ex)
            {
                result = BadRequest(new { message = ex });
            }

            return result;
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "employee, manager")]
        public async Task<ActionResult<Category>> EditCategory(
            int id,
            [FromBody] Category category,
            [FromServices] DataContext context)
        {
            if (id != category.Id)
                return NotFound(new { message = "Categoria não encontrada" });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                context.Entry<Category>(category).State = EntityState.Modified;
                await context.SaveChangesAsync();
                return Ok(category);
            }
            catch
            {
                return BadRequest(new { message = "Erro ao atualizar categoria" });
            }
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "employee, manager")]
        public async Task<ActionResult<Category>> DeleteCategory(
            int id,
            [FromServices] DataContext context)
        {
            try
            {
                var category = context.Categories
                    .FirstOrDefault(c => c.Id == id);

                if (category is null)
                    return NotFound(new { message = "Categoria não encontrada" });

                context.Categories.Remove(category);
                await context.SaveChangesAsync();
                return Ok();
            }
            catch
            {
                return BadRequest(new { message = "Não foi possível remover a categoria" });
            }
        }
    }
}