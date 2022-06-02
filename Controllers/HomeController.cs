using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Shop.Data;
using Shop.Models;

namespace Shop.Controllers
{
    [Route("v1")]
    public class HomeController : Controller
    {
        [HttpGet]
        public async Task<ActionResult<dynamic>> Get([FromServices] DataContext context)
        {
            var employee = new User { Username = "robin", Password = "robin", Role = "employee" };
            var manager = new User { Username = "batman", Password = "batman", Role = "manager" };
            var category = new Category { Title = "Equipamento de combate" };
            var product = new Product { Title = "BatArangue", Description = "Bumerangue do batman, taramram", Price = 299, Category = category };

            context.Users.AddRange(new List<User> { employee, manager });
            context.Categories.Add(category);
            context.Products.Add(product);
            await context.SaveChangesAsync();

            return Ok(new { message = "Dados Configurados" });
        }
    }
}