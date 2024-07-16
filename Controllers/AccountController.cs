using BlogApi.Data;
using BlogApi.Extensions;
using BlogApi.Models;
using BlogApi.Services;
using BlogApi.ViewModels;
using BlogApi.ViewModels.Accounts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureIdentity.Password;

namespace BlogApi.Controllers;

[ApiController]
public class AccountController : ControllerBase
{

    [HttpPost("v1/accounts/register")]
    public async Task<IActionResult> Register(
        [FromServices] BlogDataContext context,
        [FromBody] RegisterViewModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(new ResultViewModel<string>(ModelState.GetErrors()));

        var user = new User()
        {
            Id = 0,
            Name = model.Name,
            Email = model.Email,
            Slug = model.Email.Replace("@","-").Replace(".","-")
        };
        var password = PasswordGenerator.Generate(25);
        user.PasswordHash = PasswordHasher.Hash(password);
        try
        {
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();
            return Ok(new ResultViewModel<dynamic>(new
            {
                user = user.Email,password
            }));
        }
        catch (DbUpdateException)
        {
            return StatusCode(400,new ResultViewModel<string>("Erro na base de dados (Email Cadastrado)"));
        }
        catch (Exception)
        {
            return StatusCode(500,new ResultViewModel<string>("Falha interna do servidor"));
        }
        
    }
    
    
    [HttpPost("v1/accounts/login")]
    public async Task<IActionResult> Login(
        [FromServices] BlogDataContext context,
        [FromBody] LoginViewModel model,
        [FromServices] TokenService tokenService)
    {
        if (!ModelState.IsValid)
            return BadRequest(new ResultViewModel<string>(ModelState.GetErrors()));

        var user = await context.Users.AsNoTracking()
            .Include(x=>x.Roles)
            .FirstOrDefaultAsync(x => x.Email == model.Email);
        if (user == null)
            return StatusCode(401, new ResultViewModel<string>("Usuário ou senha inválidos"));

        if (!PasswordHasher.Verify(user.PasswordHash, model.Password))
            return StatusCode(401, new ResultViewModel<string>("Usuário ou senha inválidos"));
        
        try
        {
            var token = tokenService.GenerateToken(user);
            return Ok(new ResultViewModel<string>(token,null));
        }
        catch (Exception)
        {
            return StatusCode(500, new ResultViewModel<string>("Falha interna no servidor"));
        }
    }    
    
    
}