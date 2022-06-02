using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;
using Shop.Services;

namespace Shop.Controllers
{
    [Route("v1/users")]
    public class UserController : ControllerBase
    {
        [HttpGet]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult<List<User>>> GetUsers(
            [FromServices] DataContext context
        )
        {
            var users = await context.Users
                .AsNoTracking()
                .ToListAsync();

            return Ok(users);
        }

        [HttpPost]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult<User>> CreateUserAsync(
            [FromServices] DataContext context,
            [FromBody] User user)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                user.Role = "employee";
                context.Users.Add(user);
                await context.SaveChangesAsync();

                user.Password = "";
                return user;
            }
            catch
            {
                return BadRequest(new { message = "Não foi possível cria o usuário" });
            }

        }

        [HttpPost("login")]
        public async Task<ActionResult<dynamic>> AuthenticateAsync(
            [FromServices] DataContext context,
            [FromBody] User userModel)
        {
            var user = await context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(
                u => u.Username == userModel.Username &&
                u.Password == userModel.Password
            );

            if (user is null)
                return NotFound(new { message = "Usuário ou senha inválidos" });

            var token = TokenService.GenerateToken(user);
            user.Password = "";
            return new
            {
                user = user,
                token = token
            };
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult<User>> EditUser(
            [FromServices] DataContext context,
            int id,
            [FromBody] User userModel
        )
        {
            if (id != userModel.Id)
                return NotFound(new { message = "Usuário não encontrada" });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                context.Entry<User>(userModel).State = EntityState.Modified;
                await context.SaveChangesAsync();

                userModel.Password = "";
                return Ok(userModel);
            }
            catch
            {
                return BadRequest(new { message = "Erro ao atualizar categoria" });
            }
        }
    }
}