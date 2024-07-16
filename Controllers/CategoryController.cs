using System.Data.Common;
using System.Security.Claims;
using BlogApi.Data;
using BlogApi.Extensions;
using BlogApi.Models;
using BlogApi.Services;
using BlogApi.ViewModels;
using BlogApi.ViewModels.Categories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace BlogApi.Controllers;

[ApiController]
public class CategoryController : ControllerBase
{
    
    [HttpGet("v1/categories")]
    public async Task<IActionResult> GetAsync(
        [FromServices] BlogDataContext context,
        [FromServices] IMemoryCache cache)
    {
        try
        {
            var categories = cache.GetOrCreate("CategoryCache", entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
                    return GetCategories(context);
                });
            return Ok(new ResultViewModel<List<Category>>(categories));
        }
        catch (Exception)
        {
            return StatusCode(500, new ResultViewModel<string>("Erro interno no servidor"));
        }
    }

    private List<Category> GetCategories (BlogDataContext context)
    {
        var categories = context.Categories.AsNoTracking().ToList();
        return categories;
    }
    
    
    [HttpGet("v1/categories/{id:int}")]
    public async Task<IActionResult> GetByIdAsync(
        [FromServices] BlogDataContext context,
        [FromRoute] int id)
    {
        try
        {
            var category = await context.Categories.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (category == null)
                return NotFound(new ResultViewModel<string>("Categoria não encontrada"));
            
            return Ok(new ResultViewModel<Category>(category));
        }
        catch (Exception)
        {
            return StatusCode(500, new ResultViewModel<string>("Erro interno do servidor"));
        }
    }
    
    
    [HttpPost("v1/categories")]
    public async Task<IActionResult> PostAsync(
        [FromServices] BlogDataContext context,
        [FromBody] EditorCategoryViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResultViewModel<Category>(ModelState.GetErrors()));

            var category = new Category
            {
                Id = 0,
                Name = model.Name,
                Slug = model.Slug.ToLower()
            };

            await context.Categories.AddAsync(category);
            await context.SaveChangesAsync();

            return Created($"v1/categories/{category.Id}", new ResultViewModel<Category>(category));
        }
        catch (DbException)
        {
            return StatusCode(500, new ResultViewModel<string>("Não foi possível incluir a categoria"));
        }
        catch (Exception)
        {
            return StatusCode(500, new ResultViewModel<string>("Erro interno do servidor"));
        }
    }

    
    [HttpPut("v1/categories/{id:int}")]
    public async Task<IActionResult> PutAsync([FromServices] BlogDataContext context,
        [FromBody] EditorCategoryViewModel model,
        [FromRoute] int id)
    {
        try
        {
            var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);

            if (!ModelState.IsValid)
                return BadRequest();

            category.Name = model.Name;
            category.Slug = model.Slug.ToLower();

            await context.SaveChangesAsync();
            return Ok(category);
        }
        catch (DbException)
        {
            return StatusCode(500, "Erro ao atualizar a categoria");
        }
        catch (Exception)
        {
            return StatusCode(500,new ResultViewModel<string>("Erro interno do servidor"));
        }
    }

    
    
    [HttpDelete("v1/categories/{id:int}")]
    public async Task<IActionResult> DeleteAsync(
        [FromServices] BlogDataContext context,
        [FromRoute] int id)
    {
        try
        {
            var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);
            if (category == null)
                return NotFound(new ResultViewModel<string>("Categoria não encontrada"));

            context.Categories.Remove(category);
            await context.SaveChangesAsync();
            return Ok(new ResultViewModel<Category>(category));
        }
        catch (DbException)
        {
            return StatusCode(500, new ResultViewModel<string>("Erro ao excluir a categoria"));
        }
        catch (Exception)
        {
            return StatusCode(500, new ResultViewModel<string>("Erro interno de servidor"));
        }
    }
}