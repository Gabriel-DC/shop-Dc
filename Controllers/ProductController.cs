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
    [Route("v1/products")]
    public class ProductController : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<List<Product>>> GetAllProducts(
            [FromServices] DataContext context
        )
        {
            var products = await context.Products
                .Include(p => p.Category)
                .AsNoTracking()
                .ToListAsync();

            return Ok(products);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<List<Product>>> GetProductById(
            int id,
            [FromServices] DataContext context
        )
        {
            var product = await context.Products
                .Include(p => p.Category)
                .AsNoTracking()
                .SingleOrDefaultAsync(p => p.Id.Equals(id));

            return Ok(product);
        }

        [HttpGet("categories/{id:int}")]
        public async Task<ActionResult<List<Product>>> GetAllProductByCategory(
            int id,
            [FromServices] DataContext context
        )
        {
            var productsCategory = await context.Products
                .Include(p => p.Category)
                .AsNoTracking()
                .Where(p => p.CategoryId == id)
                .ToListAsync();

            return Ok(productsCategory);
        }

        [HttpPost]
        [Authorize(Roles = "employee, manager")]
        public async Task<ActionResult<List<Product>>> CreateProduct(
            [FromServices] DataContext context,
            [FromBody] Product product
        )
        {
            if (ModelState.IsValid)
            {
                context.Products.Add(product);
                await context.SaveChangesAsync();
                return Ok(product);
            }
            else
                return BadRequest(ModelState);
        }
    }
}